using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string queueName = "Download";
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "alejandro",
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            Log("connection created.");

            using var channel = connection.CreateModel();

            var listener = new MessageBusConsumerListener<string>(channel, queueName);

            var subscription = listener.Subscribe(OnNext, OnError, OnCompleted);

            Console.ReadLine();

            subscription.Dispose();
            channel.Close();
            connection.Close();


            //SubMethods

            void OnNext(Message<string> message) =>
                Log($"Received '{message.Body}' (at {message.DeliveredAt}).");

            void OnError(Exception ex) =>
                Log($"An error occured while handeling a message from the queue. {ex.Message}.");

            void OnCompleted() =>
                Log("Subscription Disposed.");

            void Log(string message) =>
                Console.WriteLine($"[{DateTimeOffset.Now}]   {message}");
        }
    }
}
