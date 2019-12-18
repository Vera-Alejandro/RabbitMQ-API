using System;

namespace Interstates.Control.MessageBus.RabbitMq.Utilities
{
    internal sealed class AnonymousDisposable : IDisposable
    {
        private readonly Action _dispose;

        public AnonymousDisposable(Action dispose)
        {
            _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
        }

        public void Dispose() => _dispose();
    }
}