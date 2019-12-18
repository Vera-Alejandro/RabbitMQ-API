using System;
using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;

namespace Send
{
    public class OtherType
    {
        public string Value { get; set; }
    }

    internal static class Program
    {
        public static void Main(string[] args)
        {
            var queueName = "strings_queue";
            Log("Application started. Connecting to RabbitMq...");
                var factory = new ConnectionFactory() 
                { 
                    HostName = "localhost",
                    UserName = "alejandro",
                    Password = "teslamotors"
                };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Enter the words to send separated by spaces.");
            var words = GetMessages(Console.ReadLine());

            var producer = new MessageBusProducerClient(channel, queueName);

            foreach (var word in words)
            {
                Log($"Sending message '{word}'...");
                var message = new Message<string>(Guid.NewGuid(), word);
                producer.PostAsync(message);
                Log("Message sent.");
            }

            Console.ReadLine();

            channel.Close();
            connection.Close();

            string[] GetMessages(string messageInput) =>
                (messageInput.Length > 0) ? messageInput.Split(' ') : new string[] { "Hello.", "Goodbye." };

            void Log(string message) =>
                Console.WriteLine($"[{DateTimeOffset.Now}]  {message}");
        }
    }
}
