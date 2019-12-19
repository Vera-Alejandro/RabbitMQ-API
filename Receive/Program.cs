using System;
using System.Linq;
using System.Threading;
using RabbitMQ.Client;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using Interstates.Control.MessageBus;
using System.Collections.Generic;

namespace Receive
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			Log("started");
			var queueName = "strings_queue";

			var factory = new ConnectionFactory() 
			{ 
				HostName = "localhost",
				UserName = "alejandro",
				Password = "teslamotors"
			};
			using var connection = factory.CreateConnection();
			Log("rabbitmq connection created");

			using var channel = connection.CreateModel();
			Log("channel created");

			var listener = new MessageBusConsumerListener<string>(channel, queueName);
			
			Log("listener created");

			var subscription = listener.Subscribe(OnNext, OnError, OnCompleted);
			Log("listener subscribed");

			Console.ReadLine();

			subscription.Dispose();
			channel.Close();
			connection.Close();

			void OnCompleted() =>
				Log("Subscription disposed.");

			void OnError(Exception ex) =>
				Log($"An error occurred while handling a message from the queue. {ex.Message} {ex.StackTrace}.");

			void OnNext(Message<string> message) =>
				Log($"Received '{message.Body}' (at {message.DeliveredAt}).");
			
			void Log(string message) => 
				Console.WriteLine($"[{DateTimeOffset.Now}]  {message}");
		}

		private static void DoStuff(string message)
		{
			int dots = message.Split('.').Count();
			Thread.Sleep(1000 * dots);
		}
	}
}
