using Confluent.Kafka;
using HotChocolate.Subscriptions;
using LaundryCleaning.Service.Common.Exceptions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Services.Dispatcher;
using LaundryCleaning.Service.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NPOI.XWPF.UserModel;

namespace LaundryCleaning.Service.Services.Background
{
    public class ReceivedBackgroundService : BackgroundService
    {
        private readonly ILogger<PublisherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConsumer<Ignore, string>? _consumer;
        private List<string> _currentTopics = new();

        public ReceivedBackgroundService(ILogger<PublisherService> logger
            , IConfiguration configuration
            , IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "group-laundry-cleaning-received",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _dispatcher = scope.ServiceProvider.GetRequiredService<SystemMessageDispatcher>();

            // Initial topic load
            var topics = await _dbContext._publisher
                        .Select(x => x.Topic)
                        .Distinct()
                        .ToListAsync(cancellationToken);

            // Case DB 0
            if (topics.Count == 0)
            {
                topics.Add("start");
                _logger.LogInformation("Start topic");
            }

            _currentTopics = topics;
            _consumer.Subscribe(_currentTopics);
            _logger.LogInformation("Subscribed to initial topics: {Topics}", string.Join(", ", _currentTopics));

            // Run monitor loop in background
            _ = Task.Run(() => MonitorTopicsAsync(cancellationToken), cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(TimeSpan.FromSeconds(5));

                    if (result == null)
                    {
                        continue;
                    }
                    
                    _logger.LogInformation("Received message from topic {Topic}: {Value}", result.Topic, result.Message.Value);

                    // Dispatch
                    var entity = new SystemReceived
                    {
                        Topic = result.Topic,
                        Payload = result.Message.Value,
                    };
                    try
                    {
                        entity.IsProcessed = true;
                        entity.ProcessedAt = DateTime.Now;

                        await _dispatcher.DispatchAsync(result.Topic, result.Message.Value, cancellationToken);

                        entity.ReceivedAt = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        entity.ErrorMessage = ex.Message;
                        _logger.LogError(ex, "Error dispatching message from topic {Topic}", result.Topic);
                    }

                    await _dbContext._received.AddAsync(entity, cancellationToken);
                    await _dbContext.SaveChangesAsync();

                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Service is stopping gracefully...");
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Consume error");
                }
            }

            _consumer?.Close();
        }

        private async Task MonitorTopicsAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var newTopics = await _dbContext._publisher
                        .Select(x => x.Topic)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    // Case DB 0
                    if (newTopics.Count == 0)
                    {
                        newTopics.Add("start");
                    }

                    if (!newTopics.SequenceEqual(_currentTopics))
                    {
                        _logger.LogInformation("Detected topic changes. Updating subscription...");

                        _consumer?.Unsubscribe();
                        _consumer?.Subscribe(newTopics);
                        _currentTopics = newTopics;

                        _logger.LogInformation("Resubscribed to: {Topics}", string.Join(", ", newTopics));
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken); // Check every 30s
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("MonitorTopicsAsync stopped gracefully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while monitoring topics");
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }

    }
}
