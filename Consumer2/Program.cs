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

            MessageBusConsumerListener<int> listener = new MessageBusConsumerListener<int>(channel, queueName);

            var subscription = listener.Subscribe(OnNext, OnError, OnCompleted);

            Console.ReadLine();

            subscription.Dispose();
            channel.Close();
            connection.Close();

            //Submethods

            void OnNext(Message<int> message)
            {
                Log($"Sleep for {message.Body} sec :: delivered at {message.DeliveredAt}.");
                System.Threading.Thread.Sleep(message.Body * 1000);
            }
            void OnError(Exception ex) =>
                Log($"Error occured on delivery.{ex.Message}");
            void OnCompleted() =>
                Log("Subscription Disposed.");
            void Log(string message)
            {
                Console.WriteLine($"[{DateTimeOffset.Now}] :: {message}");
            }

        }
    }
}
