using Blaise.Queue.Contracts.Enums;
using Blaise.Queue.Contracts.Interfaces;
using Blaise.Queue.Contracts.Interfaces.Fluent;
using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using Blaise.Queue.Contracts.Models;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Services.Queue;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.Services.Queue
{
    public class SubscriptionServiceTests
    {
        private Mock<IConfigurationProvider> _configurationProviderMock;
        private Mock<IFluentQueueProvider> _fluentQueueProviderMock;
        private Mock<IFluentRegisterQueue> _fluentQueueRegisterMock;
        private Mock<IFluentSingleQueue> _fluentQueueSingleMock;
        private Mock<IMessageHandlerCallback> _messageHandlerMock;

        private readonly ConnectionConfigurationModel _configurationModel;
        private readonly string _exchangeName;

        private SubscriptionService _sut;

        public SubscriptionServiceTests()
        {
            _configurationModel = new ConnectionConfigurationModel();
            _exchangeName = "ExchangeName";
        }

        [SetUp]
        public void SetUpTests()
        {
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.Setup(c => c.GetQueueConnectionConfigurationModel()).Returns(_configurationModel);
            _configurationProviderMock.Setup(c => c.ExchangeName).Returns(_exchangeName);

            _fluentQueueProviderMock = new Mock<IFluentQueueProvider>();
            _fluentQueueRegisterMock = new Mock<IFluentRegisterQueue>();
            _fluentQueueSingleMock = new Mock<IFluentSingleQueue>();
            _fluentQueueProviderMock.Setup(q => q.WithConnection(It.IsAny<ConnectionConfigurationModel>())).Returns(_fluentQueueRegisterMock.Object);
            _fluentQueueRegisterMock.Setup(q => q.WithExchange(It.IsAny<string>(), It.IsAny<ExchangeType>(), It.IsAny<bool>())).Returns(_fluentQueueRegisterMock.Object);

            _messageHandlerMock = new Mock<IMessageHandlerCallback>();

            _sut = new SubscriptionService(_configurationProviderMock.Object, _fluentQueueProviderMock.Object, _messageHandlerMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_SubscribeToDataDeliveryQueue_Then_The_Correct_Calls_Are_Made()
        {
            //arrange
            var queueName = "QueueName";
            var routingKey = "RoutingKey";

            _configurationProviderMock.Setup(c => c.DataDevliveryQueueName).Returns(queueName);
            _configurationProviderMock.Setup(c => c.DataDevliveryRoutingKey).Returns(routingKey);

            _fluentQueueRegisterMock.Setup(q => q.WithQueue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(_fluentQueueSingleMock.Object);
            _fluentQueueSingleMock.Setup(q => q.WithSubscription(It.IsAny<IMessageHandlerCallback>())).Returns(_fluentQueueProviderMock.Object);

            //act
            _sut.SubscribeToDataDeliveryQueue();

            //assert
            _fluentQueueProviderMock.Verify(v => v.WithConnection(_configurationModel), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.WithExchange(_exchangeName, ExchangeType.direct, true), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.WithQueue(queueName, routingKey, true), Times.Once);
            _fluentQueueSingleMock.Verify(v => v.WithSubscription(_messageHandlerMock.Object), Times.Once);
        }

        [Test]
        public void Given_I_Call_CancelAllSubscriptionsAndDispose_Then_The_Correct_Calls_Are_Made()
        {
            //arrange
            _fluentQueueRegisterMock.Setup(q => q.CancelAllSubscriptions()).Returns(_fluentQueueProviderMock.Object);
            _fluentQueueProviderMock.Setup(q => q.Dispose());

            //act
            _sut.CancelAllSubscriptionsAndDispose();

            //assert
            _fluentQueueProviderMock.Verify(v => v.WithConnection(_configurationModel), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.WithExchange(_exchangeName, ExchangeType.direct, true), Times.Once);
            _fluentQueueRegisterMock.Verify(v => v.CancelAllSubscriptions(), Times.Once);
            _fluentQueueProviderMock.Verify(v => v.Dispose(), Times.Once);
        }
    }
}
