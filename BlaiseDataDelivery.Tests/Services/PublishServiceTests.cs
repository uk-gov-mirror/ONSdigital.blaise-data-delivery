using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.Fluent;
using Blaise.Queue.Contracts.Models;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Services;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.Services
{
    public class PublishServiceTests
    {
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IFluentQueueProvider> _fluentQueueProviderMock;
        private Mock<IFluentRegisterQueue> _fluentQueueRegisterMock;
        private Mock<IFluentSingleQueue> _fluentQueueSingleMock;

        private readonly ConnectionConfigurationModel _configurationModel;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        private PublishService _sut;

        public PublishServiceTests()
        {
            _configurationModel = new ConnectionConfigurationModel();
            _exchangeName = "ExchangeName";
            _queueName = "QueueName";
            _routingKey = "RoutingKey";
        }

        [SetUp]
        public void SetUpTests()
        {
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.Setup(c => c.GetQueueConnectionConfigurationModel()).Returns(_configurationModel);
            _configurationProviderMock.Setup(c => c.ExchangeName).Returns(_exchangeName);
            _configurationProviderMock.Setup(c => c.StatusUpdateQueueName).Returns(_queueName);
            _configurationProviderMock.Setup(c => c.StatusUpdateRoutingKey).Returns(_routingKey);

            _fluentQueueProviderMock = new Mock<IFluentQueueProvider>();
            _fluentQueueRegisterMock = new Mock<IFluentRegisterQueue>();
            _fluentQueueSingleMock = new Mock<IFluentSingleQueue>();
            _fluentQueueProviderMock.Setup(q => q.WithConnection(It.IsAny<ConnectionConfigurationModel>())).Returns(_fluentQueueRegisterMock.Object);
            _fluentQueueRegisterMock.Setup(q => q.WithExchange(It.IsAny<string>(), It.IsAny<ExchangeType>(), It.IsAny<bool>())).Returns(_fluentQueueRegisterMock.Object);

            _sut = new PublishService(_configurationProviderMock.Object, _fluentQueueProviderMock.Object);
        }

        [Test]
        public void Given_I_Call_PublishStatusUpdate_Then_The_Correct_Calls_Are_Made_To_Publish_To_The_Correct_Route()
        {
            //arrange
            var messageType = "MessageType";
            var message = "Message";

            _fluentQueueRegisterMock.Setup(q => q.WithQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(_fluentQueueSingleMock.Object);
            _fluentQueueSingleMock.Setup(q => q.Publish(It.IsAny<string>(), It.IsAny<string>())).Returns(_fluentQueueProviderMock.Object);

            //act
            _sut.PublishStatusUpdate(messageType, message);

            //assert
            _fluentQueueProviderMock.Verify(v => v.WithConnection(_configurationModel), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.WithExchange(_exchangeName, ExchangeType.direct, true), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.WithQueue(_queueName, _routingKey, true), Times.Once);
            _fluentQueueSingleMock.Verify(v => v.Publish(messageType, message), Times.Once);
        }
    }
}
