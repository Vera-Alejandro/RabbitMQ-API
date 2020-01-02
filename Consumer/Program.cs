using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace Consumer
{
	class Program
	{
		static void Main(string[] args)
		{
			const string queueName = "Download_Batching_1";
			ConnectionFactory factory = new ConnectionFactory()
			{
				HostName = "localhost",
				UserName = "alejandro",
				Password = "teslamotors"
			};

			using var connection = factory.CreateConnection();
			Log("connection created. -- Batching 1.");

			using var channel = connection.CreateModel();

			MessageBusConsumerListener<int> listener = new MessageBusConsumerListener<int>(channel, queueName);

			var subscription = listener.Subscribe(OnNext, OnError, OnCompleted);

			Console.ReadLine();

			subscription.Dispose();
			channel.Close();
			connection.Close();


			//SubMethods

			void OnNext(Message<int> message)
			{
				Log($"Sleep for {message.Body} sec :: delivered at {message.DeliveredAt}.");
				Thread.Sleep(message.Body * 1000);
			}

			void OnError(Exception ex) =>
				Log($"An error occured while handeling a message from the queue. {ex.Message}.");

			void OnCompleted() =>
				Log("Subscription Disposed.");

			void Log(string message)
			{
				Console.WriteLine($"[{DateTimeOffset.Now}]   {message}");
			}
		}
	}
}
