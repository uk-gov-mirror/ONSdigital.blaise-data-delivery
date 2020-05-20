using Blaise.Nuget.PubSub.Contracts.Interfaces;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Queue;

namespace BlaiseDataDelivery.Services.Queue
{
    public class QueueService : IQueueService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFluentQueueApi _queueProvider;
        private readonly IMessageHandler _messageHandler;

        public QueueService(
            IConfigurationProvider configurationProvider,
            IFluentQueueApi queueProvider,
            IMessageHandler messageHandler)
        {
            _configurationProvider = configurationProvider;
            _queueProvider = queueProvider;
            _messageHandler = messageHandler;
        }

        public void Subscribe()
        {
            _queueProvider
                .ForProject(_configurationProvider.ProjectId)
                .ForSubscription(_configurationProvider.SubscriptionId)
                .StartConsuming(_messageHandler);
        }

        public void CancelAllSubscriptions()
        {
            _queueProvider
                .StopConsuming();
        }
    }
}
