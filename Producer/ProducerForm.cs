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
            string sendingMessage = $"[{DateTimeOffset.Now}]   Hey I want you to download this.";

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "alejandro", 
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            MessageBusProducerClient producer = new MessageBusProducerClient(channel, queueName);

            Message<string> message  = new Message<string>(Guid.NewGuid(), sendingMessage);

            int.TryParse(Download1Count.Text, out int count);
            this.Enabled = false;

            
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    producer.PostAsync(message);
                    Debug.WriteLine("Message Sent");
                }
            }
            else
            {
                producer.PostAsync(message);
                Debug.WriteLine("Message Sent");
            }

            this.Enabled = true;
            channel.Close();
            connection.Close();
        }

        private void Batching2_Click(object sender, EventArgs e)
        {
            string queueName = "Download_Batching_2";
            string sendingMessage = $"[{DateTimeOffset.Now}]   Download this yo.";

            ConnectionFactory factory = new ConnectionFactory()
            { 
                HostName = "localhost",
                UserName = "alejandro",
                Password = "teslamotors"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            MessageBusProducerClient producer = new MessageBusProducerClient(channel, queueName);

            Message<string> message = new Message<string>(Guid.NewGuid(), sendingMessage);

            int.TryParse(Download1Count.Text, out int count);

            this.Enabled = false;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    producer.PostAsync(message);
                    Debug.WriteLine("Message Sent");
                }
            } 
            else
            {
                producer.PostAsync(message);
                Debug.WriteLine("Message Sent");
            }

            Debug.WriteLine($"Message sent at {DateTimeOffset.Now}");

            this.Enabled = true;
            channel.Close();
            connection.Close();
        }
    }
}
