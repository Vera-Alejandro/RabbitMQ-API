using System;
using System.Collections.Generic;
using System.Text;

namespace Interstates.Control.MessageBus.RabbitMq
{
    internal interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
