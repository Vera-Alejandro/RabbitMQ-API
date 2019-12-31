using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Producer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Batching1_Click(object sender, EventArgs e)
        {
            const string queueName = "Download_Batching_1";
            Random rand = new Random();
            int sendingMessage;

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "alejandro", 
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            MessageBusProducerClient producer = new MessageBusProducerClient(channel, queueName);

            int.TryParse(Download1Count.Text, out int count);
            
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    sendingMessage = rand.Next(1, 30);
                    Message<int> message  = new Message<int>(Guid.NewGuid(), sendingMessage);
                    producer.PostAsync(message);
                    Debug.WriteLine("Message Sent");
                }
            }
            else
            {
                sendingMessage = rand.Next(1, 30);
                Message<int> message = new Message<int>(Guid.NewGuid(), sendingMessage);
                producer.PostAsync(message);
                Debug.WriteLine("Message Sent");
            }

            channel.Close();
            connection.Close();
        }

        private void Batching2_Click(object sender, EventArgs e)
        {
            string queueName = "Download_Batching_2";
            Random rand = new Random();
            int sendingMessage = rand.Next(1, 30);

            ConnectionFactory factory = new ConnectionFactory()
            { 
                HostName = "localhost",
                UserName = "alejandro",
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            MessageBusProducerClient producer = new MessageBusProducerClient(channel, queueName);

            int.TryParse(Download1Count.Text, out int count);

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Message<int> message = new Message<int>(Guid.NewGuid(), sendingMessage);
                    producer.PostAsync(message);
                    Debug.WriteLine("Message Sent");
                }
            } 
            else
            {
                Message<int> message = new Message<int>(Guid.NewGuid(), sendingMessage);
                producer.PostAsync(message);
                Debug.WriteLine("Message Sent");
            }

            Debug.WriteLine($"Message sent at {DateTimeOffset.Now}");

            channel.Close();
            connection.Close();
        }
    }
}
