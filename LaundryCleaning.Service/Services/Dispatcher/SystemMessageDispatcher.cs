using LaundryCleaning.Service.Services.Attributes;
using LaundryCleaning.Service.Services.Interfaces;
using System.Reflection;
using System.Text.Json;

namespace LaundryCleaning.Service.Services.Dispatcher
{
    public class SystemMessageDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SystemMessageDispatcher> _logger;
        private readonly Dictionary<string, Type> _topicTypeMap;

        public SystemMessageDispatcher(IServiceScopeFactory scopeFactory
            , ILogger<SystemMessageDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _topicTypeMap = DiscoverTopicTypeMap();
        }

        private Dictionary<string, Type> DiscoverTopicTypeMap()
        {
            var map = new Dictionary<string, Type>();
            var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    return ex.Types.Where(t => t != null)!; // ambil yang berhasil dimuat
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .Where(t => t.GetCustomAttribute<SystemMessageHandlerForAttribute>() != null)
            .ToList();

            foreach (var handler in handlerTypes)
            {
                var attr = handler.GetCustomAttribute<SystemMessageHandlerForAttribute>();
                _logger.LogInformation("Mapping topic '{Topic}' to handler '{Handler}'", attr.TopicName, handler.FullName);

                var interfaceType = handler.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISystemMessageHandler<>));

                var messageType = interfaceType?.GetGenericArguments().FirstOrDefault();
                if (messageType != null && !string.IsNullOrWhiteSpace(attr?.TopicName))
                {
                    map[attr.TopicName] = messageType;
                }
            }

            foreach (var pair in map)
            {
                _logger.LogInformation("Registered topic: {Topic} -> {Type}", pair.Key, pair.Value.Name);
            }

            return map;
        }

        public async Task DispatchAsync(string topic, string json, CancellationToken cancellationToken)
        {
            if (!_topicTypeMap.TryGetValue(topic, out var messageType))
            {
                _logger.LogWarning("No handler found for topic {Topic}", topic);
                return;
            }

            var payload = JsonSerializer.Deserialize(json, messageType);

            using var scope = _scopeFactory.CreateScope();
            var handlerType = typeof(ISystemMessageHandler<>).MakeGenericType(messageType);
            var handler = scope.ServiceProvider.GetRequiredService(handlerType);

            if (handler == null) return;

            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                await (Task)method.Invoke(handler, new[] { payload!, cancellationToken })!;
            }
        }
    }
}
