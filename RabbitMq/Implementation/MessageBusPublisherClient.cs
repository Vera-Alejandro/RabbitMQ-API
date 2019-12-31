using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interstates.Control.MessageBus.RabbitMq.Contracts;
using RabbitMQ.Client;

namespace Interstates.Control.MessageBus.RabbitMq.Implementation
{
    public class MessageBusPublisherClient : IMessageBusClient
    {
        private readonly ITimeProvider _timeProvider;
        private readonly ISerializer _serializer;
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public MessageBusPublisherClient(
            IModel channel,
            string exchangeName)
            : this(
                  new DefaultTimeProvider(),
                  new JsonSerializer(Resources.DefaultJsonSerializerSettings),
                  channel,
                  exchangeName)
        {
        }

        internal MessageBusPublisherClient(
            ITimeProvider timeProvider,
            ISerializer serializer,
            IModel channel,
            string exchangeName)
        {
            if (String.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Exchange name cannot be null, empty, or whitespace.", nameof(exchangeName)); // TODO: is this true?
            }

            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _exchangeName = exchangeName;
        }

        public Task PostAsync<TPayload>(Message<TPayload> message)
        {
            var properties = _channel.CreateBasicProperties();
            //properties.Headers = CreateHeaders<TPayload>();
            properties.Persistent = true;

            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Headers);

            var dto = new MessageDto
            {
                Id = message.Id,
                CorrelationId = message.CorrelationId,
                Body = _serializer.Serialize(message.Body),
                QueuedAt = _timeProvider.Now,
                DeliveredAt = null,
            };

            var bytes = _serializer.Serialize(dto);

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: String.Empty,
                basicProperties: properties,
                body: bytes);

            return Task.CompletedTask;
        }

        //private static readonly ConcurrentDictionary<Type, Lazy<PropertyInfo[]>> PropertyInformationCache = 
        //    new ConcurrentDictionary<Type, Lazy<PropertyInfo[]>>();

        private static IDictionary<string, object> CreateHeaders<TPayload>()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("x-match", "any");
            properties.Add("x-type", typeof(TPayload).Name);
            return properties;



            //var type = typeof(TPayload);
            //var properties = PropertyInformationCache.GetOrAdd(type, GetProperties).Value;
            //return properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(body, null));

            //Lazy<PropertyInfo[]> GetProperties(Type t)
            //{
            //    return new Lazy<PropertyInfo[]>(() => t
            //        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            //        .Where(prop => !prop.GetIndexParameters().Any())
            //        .ToArray());
            //}
        }
    }
}