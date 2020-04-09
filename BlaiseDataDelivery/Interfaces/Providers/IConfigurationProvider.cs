
using Blaise.Queue.Contracts.Models;

namespace BlaiseDataDelivery.Interfaces.Providers
{
    public interface IConfigurationProvider
    {
        string ExchangeName { get; }

        string DataDeliveryQueueName { get; }

        string DataDeliveryRoutingKey { get; }

        string FilePattern { get; }

        string BucketName { get; }

        string CloudStorageKey { get; }

        string EncryptionKey { get; }

        ConnectionConfigurationModel GetQueueConnectionConfigurationModel();

        QueueConfigurationModel GetQueueConfigurationModel();

        ExchangeConfigurationModel GetExchangeConfigurationModel();
    }
}
