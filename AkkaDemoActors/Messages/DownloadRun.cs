using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaDemoActors.Messages
{
    public sealed class DownloadRun// : DownloaderMessage
    {
        public DownloadRun(string systemId, int runId)
        {
            if (String.IsNullOrWhiteSpace(systemId))
            {
                throw new ArgumentException("SystemId cannot be null, empty, or whitespace.", nameof(systemId));
            }

            SystemId = systemId;
            RunId = runId;
        }

        public string SystemId { get; }
        public int RunId { get; }
    }
}
