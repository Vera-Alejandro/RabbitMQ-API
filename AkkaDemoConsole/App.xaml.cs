using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AkkaDemoConsole.ViewModels;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;

namespace AkkaDemoConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ExchangeName = "downloader-exchange";

        private IConnection Connection;
        private IModel Channel;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost"
                };
                Connection = factory.CreateConnection();
                Channel = Connection.CreateModel();

                var messageBusClient = new MessageBusPublisherClient(Channel, ExchangeName);

                MainWindow = new MainWindow();
                MainViewModel mainViewModel = new MainViewModel(messageBusClient);
                MainWindow.DataContext = mainViewModel;
                MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while starting up the application. {ex.Message}");
                Shutdown();
            }
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Connection.Dispose();
            Channel.Dispose();
        }
    }
}
