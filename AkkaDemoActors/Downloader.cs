using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Akka.Actor;
using Akka.Event;
using AkkaDemoActors.Messages;

namespace AkkaDemoActors
{
    public class Downloader : UntypedActor
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        public Downloader(string systemId, string downloaderId)
        {
            SystemId = systemId;
            DownloaderId = downloaderId;
            IsBusy = false;
        }

        public static Props Props(string groupId, string deviceId) =>
            Akka.Actor.Props.Create(() => new Downloader(groupId, deviceId));

        public string SystemId { get; }
        public string DownloaderId { get; }
        public bool IsBusy { get; private set; }
        protected override void PreStart() => _log.Info($"Device actor {SystemId}-{DownloaderId} started");
        protected override void PostStop() => _log.Info($"Device actor {SystemId}-{DownloaderId} stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DownloadRun download:
                    var isSuccessful = DownloadRun(download.RunId);
                    Sender.Tell(new DownloadResponse(download.SystemId, download.RunId, isSuccessful));
                    break;
            }
        }

        private bool DownloadRun(int runId)
        {
            Console.WriteLine($"Downloader {DownloaderId} is downloading run {runId} to system {SystemId}...");
            Thread.Sleep(2500);
            Console.WriteLine("Download Complete");
            return true;
        }

    }
}
