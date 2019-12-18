using System;

namespace Interstates.Control.MessageBus
{
    public class Message<TPayload>
    {
        public Message(
            Guid correlationId, 
            TPayload body)
        {
            Id = Guid.NewGuid();
            CorrelationId = correlationId;
            Body = body;
            QueuedAt = null;
            DeliveredAt = null;
        }

        public Message(
            Guid id,
            Guid correlationId,
            TPayload body,
            DateTimeOffset? queuedAt,
            DateTimeOffset? deliveredAt)
        {
            Id = id;
            CorrelationId = correlationId;
            Body = body;
            QueuedAt = queuedAt;
            DeliveredAt = deliveredAt;
        }

        public Guid Id { get; }
        public Guid CorrelationId { get; }
        public TPayload Body { get; }
        public DateTimeOffset? QueuedAt { get; }
        public DateTimeOffset? DeliveredAt { get; }

        public Message<TPayload> WithQueuedAt(DateTimeOffset timestamp)
        {
            return new Message<TPayload>(
                Id,
                CorrelationId,
                Body,
                timestamp,
                DeliveredAt
            );
        }

        public Message<TPayload> WithDeliveredAt(DateTimeOffset timestamp)
        {
            return new Message<TPayload>(
                Id,
                CorrelationId,
                Body,
                QueuedAt,
                timestamp
            );
        }

        public Message<TPayload> WithBody(TPayload body)
        {
            return new Message<TPayload>(
                Guid.NewGuid(),
                CorrelationId,
                body,
                QueuedAt,
                DeliveredAt
            );
        }
    }

    public class SomePayload
    {
        public string Value { get; set; }
        public string Level { get; set; }
    }
}