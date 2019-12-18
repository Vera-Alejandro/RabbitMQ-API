using System;

namespace Interstates.Control.MessageBus
{
    public interface IMessageBusListener<TPayload> : IObservable<Message<TPayload>>
    {
        IDisposable Subscribe(Action<Message<TPayload>> onNext, Action<Exception> onError, Action onCompleted);
    }

}