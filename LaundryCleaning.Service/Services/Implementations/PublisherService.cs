using LaundryCleaning.Service.Common.Exceptions;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.Data;
using LaundryCleaning.Service.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace LaundryCleaning.Service.Services.Implementations
{
    public class PublisherService : IPublisherService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PublisherService> _logger;
        private readonly IConfiguration _configuration;

        public PublisherService(
            ApplicationDbContext dbContext
            ,ILogger<PublisherService> logger
            ,IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"]
            };

            var payload = JsonSerializer.Serialize(message);

            var entity = new SystemPublisher
            {
                Topic = typeof(T).Name,
                Payload = payload
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: entity.Topic,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(entity.Payload);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: entity.Topic,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body);

                _logger.LogInformation("Published to queue {Topic}: {Payload}", entity.Topic, entity.Payload);

                entity.IsPublished = true;
                entity.PublishedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                entity.ErrorMessage = ex.Message;
                _logger.LogError(ex, "RabbitMQ publishing error");
                throw new BusinessLogicException("RabbitMQ publish failed: " + ex.Message);
            }

            await _dbContext._publisher.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}
