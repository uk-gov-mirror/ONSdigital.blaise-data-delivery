
using BlaiseDataDelivery.Interfaces.Services;
using BlaiseDataDelivery.Interfaces.Services.Queue;
using log4net;

namespace BlaiseDataDelivery.Services
{
    public class InitialiseDeliveryService : IInitialiseDeliveryService
    {
        private readonly ILog _logger;
        private readonly ISubscriptionService _subscriptionService;
       
        public InitialiseDeliveryService(
            ILog logger,
            ISubscriptionService subscriptionService)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }

        public void SetupSubscription()
        {
            _subscriptionService.SubscribeToDataDeliveryQueue();
            _logger.Info("Subscription to data delivery queue setup");

        }

        public void CancelSubscription()
        {
            _subscriptionService.CancelAllSubscriptionsAndDispose();
            _logger.Info("Subscription to data delivery queue cancelled");
        }
    }
}
