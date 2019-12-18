using System;

namespace Interstates.Control.MessageBus.RabbitMq.Utilities
{
    internal sealed class AnonymousObserver<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action OnCompleted)
        {
            _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
            _onError = onError ?? throw new ArgumentNullException(nameof(onError));
            _onCompleted = OnCompleted ?? throw new ArgumentNullException(nameof(OnCompleted));
        }

        public void OnNext(T value) => _onNext.Invoke(value);

        public void OnError(Exception error) => _onError.Invoke(error);

        public void OnCompleted() => _onCompleted.Invoke();

    }
}