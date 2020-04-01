
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;

namespace BlaiseDataDelivery.Interfaces.Services
{
    public interface ISubscriptionService
    {
        void SubscribeToDataDeliveryQueue();

        void CancelAllSubscriptionsAndDispose();
    }
}
