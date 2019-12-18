using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Consumer_Producer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void Download_Click(object sender, EventArgs e)
        {
            string queueName = "Download";
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

            var message = new Message<string>(Guid.NewGuid(), sendingMessage);

            producer.PostAsync(message);
            Debug.WriteLine("Message Sent");

            channel.Close();
            connection.Close();
        }
    }
}
