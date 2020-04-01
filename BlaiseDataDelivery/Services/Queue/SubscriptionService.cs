using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Queue;

namespace BlaiseDataDelivery.Services.Queue
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFluentQueueProvider _queueProvider;
        private readonly IMessageHandlerCallback _messageHandlerCallback;

        public SubscriptionService(
            IConfigurationProvider configurationProvider,
            IFluentQueueProvider queueProvider,
            IMessageHandlerCallback messageHandlerCallback)
        {
            _configurationProvider = configurationProvider;
            _queueProvider = queueProvider;
            _messageHandlerCallback = messageHandlerCallback;
        }

        public void SubscribeToDataDeliveryQueue()
        {
            _queueProvider
                .WithConnection(_configurationProvider.GetQueueConnectionConfigurationModel())
                .WithExchange(_configurationProvider.ExchangeName, ExchangeType.direct, true)
                .WithQueue(_configurationProvider.DataDevliveryQueueName, _configurationProvider.DataDevliveryRoutingKey, true)
                .WithSubscription(_messageHandlerCallback);
        }

        public void CancelAllSubscriptionsAndDispose()
        {
            _queueProvider
                .WithConnection(_configurationProvider.GetQueueConnectionConfigurationModel())
                .WithExchange(_configurationProvider.ExchangeName, ExchangeType.direct, true)
                .CancelAllSubscriptions()
                .Dispose();
        }
    }
}
