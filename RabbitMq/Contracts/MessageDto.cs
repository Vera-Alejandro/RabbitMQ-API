using System;

namespace Interstates.Control.MessageBus.RabbitMq.Contracts
{
    internal class MessageDto
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public byte[] Body { get; set; }
        public DateTimeOffset? QueuedAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }
    }
}
