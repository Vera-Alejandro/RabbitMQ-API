using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.Event;
using AkkaDemoActors.Messages;

namespace AkkaDemoActors
{
    public sealed class SupervisorActor : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> _downloaderActors;
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public SupervisorActor(string systemId)
        {
            SystemId = systemId;
            _downloaderActors = new Dictionary<string, IActorRef>();
        }

        public string SystemId { get; }
        public static Props Props(string systemId) => Akka.Actor.Props.Create<SupervisorActor>(systemId);
        protected override void PreStart() => _log.Info("Supervisor started");
        protected override void PostStop() => _log.Info("Supervisor stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RegisterDownloader registerMessage when StringComparer.OrdinalIgnoreCase.Equals(registerMessage.SystemId, SystemId):
                    if (_downloaderActors.TryGetValue(registerMessage.DownloaderId, out var actor))
                    {
                        _log.Info($"Downloader with id '{registerMessage.DownloaderId}' found.");
                    }
                    else
                    {
                        _log.Info($"Downloader with id '{registerMessage.DownloaderId}' not found. Creating new actor...");
                        var deviceActor = Context.ActorOf(Downloader.Props(registerMessage.SystemId, registerMessage.DownloaderId), $"downloader-{registerMessage.DownloaderId}");
                        _downloaderActors.Add(registerMessage.DownloaderId, deviceActor);
                    }
                    break;
                case RegisterDownloader registerMessage:
                    _log.Info($"Ignoring {nameof(RegisterDownloader)} message for system '{registerMessage.SystemId}' not found. This supervisor is responsible for {SystemId}.");
                    break;
                case DownloadRun downloadMessage when StringComparer.OrdinalIgnoreCase.Equals(downloadMessage.SystemId, SystemId):
                    // TODO: meaningful message routing...
                    var downloaderId = _downloaderActors.Keys.FirstOrDefault();
                    if (downloaderId != null)
                    {
                        _log.Info($"Downloader with id '{downloaderId}' found. Forwarding download message...");
                        _downloaderActors
                            .GetValueOrDefault(downloaderId)
                            .Forward(downloadMessage);
                    }
                    else
                    {
                        _log.Info($"Ignoring {nameof(DownloadRun)} message because no downloader instances have been registered for system {SystemId}.");
                    }
                    break;
                case DownloadRun downloadMessage:
                    _log.Info($"Ignoring {nameof(DownloadRun)} message for system '{downloadMessage.SystemId}' not found. This supervisor is responsible for {SystemId}.");
                    break;
                case DownloadResponse downloadResponse when StringComparer.OrdinalIgnoreCase.Equals(downloadResponse.SystemId, SystemId):
                    if (downloadResponse.IsSuccessful)
                    {
                        _log.Info($"Run {downloadResponse.RunId} successfully downloaded to system {downloadResponse.SystemId}.");
                    }
                    else
                    {
                        _log.Error($"Failed to download run {downloadResponse.RunId} to system {downloadResponse.SystemId}.");
                    }
                    break;
                case DownloadResponse downloadResponse:
                    _log.Info($"Ignoring {nameof(DownloadResponse)} message for system '{downloadResponse.SystemId}' not found. This supervisor is responsible for {SystemId}.");
                    break;
            }
        }
    }
}
