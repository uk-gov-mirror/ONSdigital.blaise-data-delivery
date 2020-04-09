using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Models;
using BlaiseDataDelivery.Interfaces.Providers;
using System.Configuration;

namespace BlaiseDataDelivery.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string ExchangeName => ConfigurationManager.AppSettings["RabbitExchange"]; 

        public string FilePattern => ConfigurationManager.AppSettings["FilePattern"];

        public string DataDeliveryQueueName => ConfigurationManager.AppSettings["DataDeliveryQueueName"];

        public string DataDeliveryRoutingKey => ConfigurationManager.AppSettings["DataDeliveryRoutingKey"];

        public string BucketName => ConfigurationManager.AppSettings["BucketName"];

        public string CloudStorageKey => ConfigurationManager.AppSettings["CloudStorageKey"];

        public string EncryptionKey => ConfigurationManager.AppSettings["EncryptionKey"];

        public ConnectionConfigurationModel GetQueueConnectionConfigurationModel()
        {

            var configurationModel = new ConnectionConfigurationModel
            {
                HostName = ConfigurationManager.AppSettings["RabbitHostName"],
                UserName = ConfigurationManager.AppSettings["RabbitUserName"],
                Password = ConfigurationManager.AppSettings["RabbitPassword"],
                PrefetchSize = 0,
                PrefetchCount = 1,
                Global = false
            };
            return configurationModel;
        }

        public QueueConfigurationModel GetQueueConfigurationModel()
        {
            var configurationModel = new QueueConfigurationModel
            {
            };
            return configurationModel;
        }

        public ExchangeConfigurationModel GetExchangeConfigurationModel()
        {
            var configurationModel = new ExchangeConfigurationModel
            {
                ExchangeType = ExchangeType.direct,
                Durable = true
            };
            return configurationModel;
        }
    }
}
