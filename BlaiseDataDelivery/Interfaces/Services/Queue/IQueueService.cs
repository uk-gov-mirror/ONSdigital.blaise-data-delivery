

using Blaise.Nuget.PubSub.Contracts.Interfaces;

namespace BlaiseDataDelivery.Interfaces.Services.Queue
{
    public interface IQueueService
    {
        void Subscribe(IMessageHandler messageHandler);

        void CancelAllSubscriptions();
    }
}
