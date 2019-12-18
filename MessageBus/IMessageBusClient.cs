using System.Threading.Tasks;

namespace Interstates.Control.MessageBus
{
    public interface IMessageBusClient
    {
        Task PostAsync<TPayload>(Message<TPayload> message);
    }

}