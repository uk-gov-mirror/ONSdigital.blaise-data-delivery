using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Queue;
using log4net;

namespace BlaiseDataDelivery.Services.Queue
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFluentQueueProvider _queueProvider;
        private readonly IMessageHandlerCallback _messageHandlerCallback;

        public SubscriptionService(
            ILog logger,
            IConfigurationProvider configurationProvider,
            IFluentQueueProvider queueProvider,
            IMessageHandlerCallback messageHandlerCallback)
        {
            _logger = logger;
            _configurationProvider = configurationProvider;
            _queueProvider = queueProvider;
            _messageHandlerCallback = messageHandlerCallback;
        }

        public void SubscribeToDataDeliveryQueue()
        {
            _queueProvider
                .WithConnection(_configurationProvider.GetQueueConnectionConfigurationModel())
                .WithExchange(_configurationProvider.ExchangeName, ExchangeType.direct, true)
                .WithQueue(_configurationProvider.DataDeliveryQueueName, _configurationProvider.DataDeliveryRoutingKey, true)
                .WithSubscription(_messageHandlerCallback);

            _logger.Info($"Subscription setup to queue '{_configurationProvider.DataDeliveryQueueName}'");
        }

        public void CancelAllSubscriptionsAndDispose()
        {
            _queueProvider
                .WithConnection(_configurationProvider.GetQueueConnectionConfigurationModel())
                .WithExchange(_configurationProvider.ExchangeName, ExchangeType.direct, true)
                .CancelAllSubscriptions()
                .Dispose();

            _logger.Info($"Subscription cancelled for queue '{_configurationProvider.DataDeliveryQueueName}'");
        }
    }
}
