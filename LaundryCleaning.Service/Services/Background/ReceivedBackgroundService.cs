using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Services.Dispatcher;
using LaundryCleaning.Service.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LaundryCleaning.Service.Services.Background
{
    public class ReceivedBackgroundService : BackgroundService
    {
        private readonly ILogger<PublisherService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection _connection;
        private IChannel _channel;
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
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"]
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

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
            _logger.LogInformation("Subscribed to initial topics: {Topics}", string.Join(", ", _currentTopics));

            // Run monitor loop in background
            _ = Task.Run(() => MonitorTopicsAsync(cancellationToken), cancellationToken);

            foreach (var topic in topics)
            {
                DeclareAndConsume(topic, _dbContext, _dispatcher, cancellationToken);
            }
        }

        private async void DeclareAndConsume(string topic, ApplicationDbContext _dbContext, SystemMessageDispatcher _dispatcher, CancellationToken cancellationToken)
        {
            await _channel.QueueDeclareAsync(queue: topic,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message from queue {topic}: {message}", topic, message);

                // Dispatch
                var entity = new SystemReceived
                {
                    Topic = topic,
                    Payload = message,
                };
                try
                {
                    entity.IsProcessed = true;
                    entity.ProcessedAt = DateTime.Now;

                    await _dispatcher.DispatchAsync(topic, message, cancellationToken);

                    entity.ReceivedAt = DateTime.Now;
                }
                catch (Exception ex)
                {
                    entity.ErrorMessage = ex.Message;
                    _logger.LogError(ex, "Error dispatching message from topic {Topic}", topic);
                }

                await _dbContext._received.AddAsync(entity, cancellationToken);
                await _dbContext.SaveChangesAsync();
                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: topic,
                                 autoAck: true,
                                 consumer: consumer);

            _logger.LogInformation("Subscribed to queue: {topic}", topic);
        }

        private async Task MonitorTopicsAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<SystemMessageDispatcher>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var newTopics = await db._publisher
                        .Select(x => x.Topic)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    if (newTopics.Count == 0)
                    {
                        newTopics.Add("start");
                    }

                    if (!newTopics.SequenceEqual(_currentTopics))
                    {
                        _logger.LogInformation("Detected queue changes. Updating subscriptions...");

                        foreach (var topic in newTopics.Except(_currentTopics))
                        {
                            DeclareAndConsume(topic, db, dispatcher, cancellationToken);
                        }

                        _currentTopics = newTopics;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while monitoring queues");
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }
    }

}
