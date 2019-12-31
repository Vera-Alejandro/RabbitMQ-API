using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AkkaDemoActors.Messages;
using Interstates.Control.MessageBus;

namespace AkkaDemoConsole.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IMessageBusClient _messageBusClient;

        public MainViewModel(IMessageBusClient messageBusClient)
        {
            _messageBusClient = messageBusClient ?? throw new ArgumentNullException(nameof(messageBusClient));
            Download = new RelayCommand(e => DownloadRun(System, Int32.Parse(RunId)), ce => Int32.TryParse(RunId, out var _));
        }

        public ICommand Download { get; }
        public string RunId { get; set; }
        public string System { get; set; }

        private void DownloadRun(string system, int runId)
        {
            var message = new Message<DownloadRun>(
                Guid.NewGuid(),
                new DownloadRun(system.Trim().Replace(' ', '-'), runId));
            Task.Run(() => _messageBusClient.PostAsync(message));
        }
    }
}
