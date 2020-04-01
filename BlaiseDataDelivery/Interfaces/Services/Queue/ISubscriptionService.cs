
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;

namespace BlaiseDataDelivery.Interfaces.Services.Queue
{
    public interface ISubscriptionService
    {
        void SubscribeToDataDeliveryQueue();

        void CancelAllSubscriptionsAndDispose();
    }
}
