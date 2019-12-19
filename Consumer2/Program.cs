using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;

namespace Consumer2
{
    class Program
    {
        static void Main(string[] args)
        {
            string queueName = "Download_Batching_2";

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "alejandro",
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            Log("connection created -- Batching 2.");

            MessageBusConsumerListener<string> listener = new MessageBusConsumerListener<string>(channel, queueName);

            var subscription = listener.Subscribe(OnNext, OnError, OnCompleted);

            Console.ReadLine();

            subscription.Dispose();
            channel.Close();
            connection.Close();

            //Submethods

            void OnNext(Message<string> message) =>
                Log($"Received {message.Body} at {message.DeliveredAt}.");
            void OnError(Exception ex) =>
                Log($"Error occured on delivery.{ex.Message}");
            void OnCompleted() =>
                Log("Subscription Disposed.");
            void Log(string message)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"[{DateTimeOffset.Now}] :: {message}");
            }

        }
    }
}
