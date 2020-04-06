using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Models;

namespace BlaiseDataDelivery.Interfaces.Providers
{
    public interface IConfigurationProvider
    {
        string ExchangeName { get; }

        string FilePattern { get; }

        string DataDeliveryQueueName { get; }

        string DataDeliveryRoutingKey { get; }

        string BucketName { get; }
        string CloudStorageKey { get; }

        ConnectionConfigurationModel GetQueueConnectionConfigurationModel();

        QueueConfigurationModel GetQueueConfigurationModel();

        ExchangeConfigurationModel GetExchangeConfigurationModel();
    }
}
