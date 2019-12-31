using System;
using Akka.Actor;
using AkkaDemoActors.Messages;
using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;

namespace AkkaDemoActors
{
    internal static class Program
    {
        private const string ExchangeName = "downloader-exchange";

        static void Main(string[] args)
        {
            LogMessage("Application started.");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            LogMessage("Connected to RabbitMq.");

            using var system = ActorSystem.Create("downloader-system");

            LogMessage("Enter the name of the system.");
            var systemId = Console.ReadLine().Replace(' ', '-');

            var supervisorName = $"downloader-supervisor-{systemId}";
            LogMessage($"Creating supervisor node '{supervisorName}'...");
            var downloaderSupervisor = system.ActorOf(SupervisorActor.Props(systemId), supervisorName);

            var downloaderName = $"downloader-{systemId}";
            LogMessage($"Registering downloader with name '{downloaderName}'...");
            downloaderSupervisor.Tell(new RegisterDownloader(systemId, downloaderName));

            var messageBusListener = new MessageBusSubscriberListener<DownloadRun>(channel, ExchangeName);
            var subscription = messageBusListener.Subscribe(OnNext, OnError, OnCompleted);

            Console.ReadLine();

            subscription.Dispose();
            channel.Dispose();
            connection.Dispose();


            void OnNext(Message<DownloadRun> message) => downloaderSupervisor.Tell(message.Body);
            void OnError(Exception ex) => Console.WriteLine($"Something terrible has happened! {ex.ToString()}");
            void OnCompleted() => Console.WriteLine($"Supervisor actor for system {systemId} is no longer subscribed to messages.");

            void LogMessage(string message) => Console.WriteLine($"[{DateTimeOffset.Now}]  {message}");
        }

    }
}
