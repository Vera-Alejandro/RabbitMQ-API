using System;
using System.Threading.Tasks;
using Interstates.Control.MessageBus.RabbitMq.Contracts;
using Interstates.Control.MessageBus.RabbitMq.Utilities;
using RabbitMQ.Client;

namespace Interstates.Control.MessageBus.RabbitMq.Implementation
{
    public class MessageBusProducerClient : IMessageBusClient
    {
        private readonly ITimeProvider _timeProvider;
        private readonly ISerializer _serializer;
        private readonly IModel _channel;
        private readonly string _queueName;

        public MessageBusProducerClient(
            IModel channel,
            string queueName)
            : this(new DefaultTimeProvider(), new JsonSerializer(Resources.DefaultJsonSerializerSettings), channel, queueName)
        {
        }

        internal MessageBusProducerClient(
            ITimeProvider timeProvider,
            ISerializer serializer,
            IModel channel,
            string queueName)
        {
            if (String.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Queue name cannot be null, empty, or whitespace.", nameof(queueName));
            }

            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _queueName = queueName;
        }

        public Task PostAsync<TPayload>(Message<TPayload> message)
        {
            var dto = new MessageDto
            {
                Id = message.Id,
                CorrelationId = message.CorrelationId,
                Body = _serializer.Serialize(message.Body),
                QueuedAt = _timeProvider.Now,
                DeliveredAt = null,
            };

            var bytes = _serializer.Serialize(dto);
            _channel.QueueDeclare(_queueName, true, false, false, null);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(String.Empty, _queueName, properties, bytes); 

            return Task.CompletedTask;
        }
    }

    public class QueueConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueueConfiguration"/> with the provided name and default settings (non-durable,
        /// non-exclusive, non-auto-delete, no arguments).
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        public QueueConfiguration(string name)
            : this(name, false, false, false, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="QueueConfiguration"/> with the provided settings.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        /// <param name="durable">Whether the queue will survive a broker restart.</param>
        /// <param name="exclusive">Whether the queue will be used by only one connection and deleted when the connection closes.</param>
        /// <param name="autoDelete">Whether the queue is deleted when the last consumer unsubscribes.</param>
        /// <param name="arguments">Optional features such as message time to live, queue length limit, priorities, etc.</param>
        public QueueConfiguration(
            string name,
            bool durable,
            bool exclusive,
            bool autoDelete,
            System.Collections.Generic.IDictionary<string, object> arguments)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Queue name cannot be null, empty, or whitespace.", nameof(name));
            }

            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = arguments;
        }

        public string Name { get; }
        public bool Durable { get; }
        public bool Exclusive { get; }
        public bool AutoDelete { get; }
        public System.Collections.Generic.IDictionary<string, object> Arguments { get; }
    }

    public class PublishConfiguration
    {
        public PublishConfiguration(
            string exchangeName,
            string routingKey,
            IBasicProperties properties)
        {
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
            Properties = properties;
        }

        public string ExchangeName { get; }
        public string RoutingKey { get; }
        public IBasicProperties Properties { get; }
    }
}