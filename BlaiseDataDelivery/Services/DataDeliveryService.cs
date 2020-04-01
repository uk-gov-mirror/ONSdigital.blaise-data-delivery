
using BlaiseDataDelivery.Interfaces.Services;

namespace BlaiseDataDelivery.Services
{
    public class DataDeliveryService : IDataDeliveryService
    {
        private readonly ISubscriptionService _subscriptionService;
        public DataDeliveryService(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
