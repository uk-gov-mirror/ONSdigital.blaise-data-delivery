using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Models;

namespace BlaiseDataDelivery.Interfaces.Providers
{
    public interface IConfigurationProvider
    {
        string ExchangeName { get; }

        string FilePattern { get; }

        string DataDevliveryQueueName { get; }

        string DataDevliveryRoutingKey { get; }

        string StatusUpdateQueueName { get; }

        string StatusUpdateRoutingKey { get; }

        ConnectionConfigurationModel GetQueueConnectionConfigurationModel();

        QueueConfigurationModel GetQueueConfigurationModel();

        ExchangeConfigurationModel GetExchangeConfigurationModel();
    }
}
