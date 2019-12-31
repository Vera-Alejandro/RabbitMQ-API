using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaDemoActors.Messages
{
    public sealed class DownloadResponse// : DownloaderMessage
    {
        public DownloadResponse(string systemId, int runId, bool isSuccessful)
        {
            if (String.IsNullOrWhiteSpace(systemId))
            {
                throw new ArgumentException("SystemId cannot be null, empty, or whitespace.", nameof(systemId));
            }

            SystemId = systemId;
            RunId = runId;
            IsSuccessful = isSuccessful;
        }

        public string SystemId { get; }
        public int RunId { get; }
        public bool IsSuccessful { get; }
    }
}
