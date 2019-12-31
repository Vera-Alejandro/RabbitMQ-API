using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaDemoActors.Messages
{
    public sealed class RegisterDownloader// : DownloaderMessage
    {
        public RegisterDownloader(string systemId, string downloaderId)
        {
            if (String.IsNullOrWhiteSpace(systemId))
            {
                throw new ArgumentException("SystemId cannot be null, empty, or whitespace.", nameof(systemId));
            }

            if (String.IsNullOrWhiteSpace(downloaderId))
            {
                throw new ArgumentException("DownloaderId cannot be null, empty, or whitespace.", nameof(downloaderId));
            }

            SystemId = systemId;
            DownloaderId = downloaderId;
        }

        public string SystemId { get; }
        public string DownloaderId { get; }
    }
}
