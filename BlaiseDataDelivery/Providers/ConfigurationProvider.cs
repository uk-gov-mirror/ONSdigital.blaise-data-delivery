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

        public string DataDevliveryQueueName => ConfigurationManager.AppSettings["DataDeliveryQueueName"];

        public string DataDevliveryRoutingKey => ConfigurationManager.AppSettings["DataDevliveryRoutingKey"];

        public string StatusUpdateQueueName => ConfigurationManager.AppSettings["StatusUpdateQueueName"];

        public string StatusUpdateRoutingKey => ConfigurationManager.AppSettings["StatusUpdateRoutingKey"];

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
