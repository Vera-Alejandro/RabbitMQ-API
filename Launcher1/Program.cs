using Interstates.Control.MessageBus;
using Interstates.Control.MessageBus.RabbitMq.Implementation;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using MessageType;
using Newtonsoft.Json;

namespace Launcher1
{
	class Program
	{
		static void Main(string[] args)
		{
			string exchangeName = "queueLink";
			Random rand = new Random();

			ConnectionFactory factory = new ConnectionFactory()
			{
				HostName = "localhost",
				UserName = "alejandro",
				Password = "teslamotors"
			};

			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			MessageBusSubscriberListener<UpdateInformation> listener = new MessageBusSubscriberListener<UpdateInformation>(channel, exchangeName);

			Console.WriteLine($"[*]  Waiting for messages ...");
			var subscriber = listener.Subscribe(OnNext, OnError, OnCompleted);
			Console.ReadLine();

			subscriber.Dispose();
			connection.Close();
			channel.Close();

			//SubMethods

			void OnNext(Message<UpdateInformation> message)
			{
				string Json = string.Empty;
				Uri informationUrl = new Uri(message.Body.InformationEndPoint);
				Uri downloadUrl = new Uri(message.Body.DownloadEndPoint);

				Console.WriteLine($"[*]  Message Received.");
				Console.WriteLine("[*]  Grabbing the launch file");


				try
				{
					//getting file information from server
					using WebClient client = new WebClient();

					var json = client.DownloadString(informationUrl);

					FIleInformation fileInfo = JsonConvert.DeserializeObject<FIleInformation>(json);

					Console.WriteLine("[*][-]  Got information from server.");


					string baseFolder = @$"C:\setups";
					string downloadFile = Path.Combine(baseFolder, message.Body.Id, fileInfo.Version, fileInfo.Name);

					Directory.CreateDirectory(Path.GetDirectoryName(downloadFile));

					//downloading file from server
					client.DownloadFile(downloadUrl, downloadFile);

					Console.WriteLine("[*]  Starting launcher downloaded from server.");
					Process newProc = new Process();
					newProc.StartInfo.FileName = downloadFile;
					newProc.Start();

				}
				catch (Exception)
				{
					throw;
				}
			}

			void OnError(Exception ex) =>
				Console.WriteLine($"Error occured, on delivery. :: {ex.Message}");
			void OnCompleted() =>
				Console.WriteLine("Subscription Disposed.");
		}
	}
}
