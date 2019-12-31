using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using MessageType;
using RabbitMQ.Client;
using System;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            string exchangeName = "queueLink";

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "alejandro",
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            {
                MessageBusPublisherClient publisher = new MessageBusPublisherClient(channel, exchangeName);

                string exitCode = "";
                while (exitCode != "-1")
                {
                    string id = "";
                    Console.WriteLine("Enter download ID");
                    id = Console.ReadLine().ToLower();

                    var updateMessage = new UpdateInformation
                    {
                        DownloadEndPoint = @$"http://localhost:57308/updates/{id}/download",
                        InformationEndPoint = @$"http://localhost:57308/updates/{id}/information",
                        Id = id
                    };

                    Message<UpdateInformation> message = new Message<UpdateInformation>(Guid.NewGuid(), updateMessage);

                    publisher.PostAsync(message);
                    Console.WriteLine($"Message Send :: {message.Body}\n\n");
                }
            }
        }
    }
}
