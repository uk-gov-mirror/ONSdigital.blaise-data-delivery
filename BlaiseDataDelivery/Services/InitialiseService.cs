
using BlaiseDataDelivery.Interfaces.Services;
using BlaiseDataDelivery.Interfaces.Services.Queue;
using log4net;

namespace BlaiseDataDelivery.Services
{
    public class InitialiseService : IInitialiseService
    {
        private readonly ILog _logger;
        private readonly IQueueService _queueService;
       
        public InitialiseService(
            ILog logger,
            IQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }

        public void Start()
        {
            _logger.Info("Subscribing to topic");
            _queueService.Subscribe();
            _logger.Info("Subscription setup");

        }

        public void Stop()
        {
            _logger.Info("Stopping subscription to topic - updated");
            _queueService.CancelAllSubscriptions();
            _logger.Info("Subscription stopped - updated");
        }
    }
}
