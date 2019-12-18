using System;
using System.Collections.Generic;
using System.Text;

namespace Interstates.Control.MessageBus.RabbitMq
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
    }
}
