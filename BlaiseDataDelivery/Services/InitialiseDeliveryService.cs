
using BlaiseDataDelivery.Interfaces.Services;
using BlaiseDataDelivery.Interfaces.Services.Queue;

namespace BlaiseDataDelivery.Services
{
    public class InitialiseDeliveryService : IInitialiseDeliveryService
    {
        private readonly ISubscriptionService _subscriptionService;
        public InitialiseDeliveryService(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void SetupSubscription()
        {
            _subscriptionService.SubscribeToDataDeliveryQueue();
        }

        public void CancelSubscription()
        {
            _subscriptionService.CancelAllSubscriptionsAndDispose();
        }
    }
}
