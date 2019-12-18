using System;

namespace Interstates.Control.MessageBus.RabbitMq.Implementation
{
    internal class DefaultTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now  => DateTimeOffset.Now;
    }
}
