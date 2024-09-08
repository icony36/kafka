

using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
        {
            _config = config.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic)
        {
            // Create consumer
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            // Subscribe to specific topic
            consumer.Subscribe(topic);

            while (true)
            {
                var consumerResult = consumer.Consume();

                if (consumerResult?.Message == null) continue;

                // Use our custom Json Converter
                var options = new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter() }
                };

                // Deserialize event
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);

                // Get the method that handle the event
                var handlerMethod = _eventHandler.GetType().GetMethod("On", [@event.GetType()]);

                if (handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method.");
                }

                // Handle the event by invoking the handler method
                handlerMethod.Invoke(_eventHandler, [@event]);

                // Tell Kafka the event has been handled, increment the log offset
                consumer.Commit(consumerResult);
            }

        }
    }
}
