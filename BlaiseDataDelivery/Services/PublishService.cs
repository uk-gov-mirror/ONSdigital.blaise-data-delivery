using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Interfaces;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services;

namespace BlaiseDataDelivery.Services
{
    public class PublishService : IPublishService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFluentQueueProvider _queueProvider;

        public PublishService(
            IConfigurationProvider configurationProvider,
            IFluentQueueProvider queueProvider)
        {
            _configurationProvider = configurationProvider;
            _queueProvider = queueProvider;
        }

        public void PublishStatusUpdate(string messageType, string message)
        {
            _queueProvider
                .WithConnection(_configurationProvider.GetQueueConnectionConfigurationModel())
                .WithExchange(_configurationProvider.ExchangeName, ExchangeType.direct, true)
                .WithQueue(_configurationProvider.StatusUpdateQueueName, _configurationProvider.StatusUpdateRoutingKey, true)
                .Publish(messageType, message);
        }
    }
}
