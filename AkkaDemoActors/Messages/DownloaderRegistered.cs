namespace AkkaDemoActors.Messages
{
    public sealed class DownloaderRegistered// : DownloaderMessage
    {
        public static DownloaderRegistered Instance { get; } = new DownloaderRegistered();
        private DownloaderRegistered() { }
    }
}
