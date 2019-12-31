using System;
using System.Collections.Generic;
using Interstates.Control.MessageBus.RabbitMq.Contracts;
using Interstates.Control.MessageBus.RabbitMq.Utilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Interstates.Control.MessageBus.RabbitMq.Implementation
{
	public class MessageBusSubscriberListener<TPayload> : IMessageBusListener<TPayload>
	{
		private readonly ITimeProvider _timeProvider;
		private readonly ISerializer _serializer;
		private readonly IModel _channel;
		private readonly string _exchangeName;

		public MessageBusSubscriberListener(
			IModel channel,
			string exchangeName)
			: this(
				  new DefaultTimeProvider(),
				  new JsonSerializer(Resources.DefaultJsonSerializerSettings),
				  channel,
				  exchangeName)
		{
		}

		internal MessageBusSubscriberListener(
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


		public IDisposable Subscribe(
			Action<Message<TPayload>> onNext,
			Action<Exception> onError,
			Action onCompleted)
		{
			if (onNext is null)
			{
				throw new ArgumentNullException(nameof(onNext));
			}

			if (onError is null)
			{
				throw new ArgumentNullException(nameof(onError));
			}

			if (onCompleted is null)
			{
				throw new ArgumentNullException(nameof(onCompleted));
			}

			return CreateSubscription(new AnonymousObserver<Message<TPayload>>(onNext, onError, onCompleted));
		}

		public IDisposable Subscribe(IObserver<Message<TPayload>> observer)
		{
			if (observer is null)
			{
				throw new ArgumentNullException(nameof(observer));
			}

			return CreateSubscription(observer);
		}

        private IDisposable CreateSubscription(IObserver<Message<TPayload>> observer)
        {
            //var headers = CreateHeaders();

            _channel.ExchangeDeclare(
                _exchangeName,
                ExchangeType.Headers);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(
                queueName,
                _exchangeName,
                String.Empty,
                headers);

			var consumer = new EventingBasicConsumer(_channel); // TODO: inject?
			consumer.Received += OnNext;
			_channel.BasicConsume(queueName, false, consumer);

			return new AnonymousDisposable(Unsubscribe);

            void OnNext(object sender, BasicDeliverEventArgs e)
            {
                try
                {
                    var dto = _serializer.Deserialize<MessageDto>(e.Body);
                    var message = new Message<TPayload>(
                        dto.Id,
                        dto.CorrelationId,
                        _serializer.Deserialize<TPayload>(dto.Body),
                        dto.QueuedAt,
                        _timeProvider.Now);

					observer.OnNext(message);

					consumer.Model.BasicAck(e.DeliveryTag, false); // TODO: evaluate true/false for param "multiple"
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
				}
			}

			void Unsubscribe()
			{
				consumer.Received -= OnNext;
				observer.OnCompleted();
			}
		}

        private static IDictionary<string, object> CreateHeaders<TPayload>()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("x-match", "all");
            properties.Add("Type", typeof(TPayload).Name);
            return properties;
        }
    }
}
