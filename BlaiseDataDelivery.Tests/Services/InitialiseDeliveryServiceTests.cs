using BlaiseDataDelivery.Interfaces.Services.Queue;
using BlaiseDataDelivery.Services;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.Services
{
    public class InitialiseDeliveryServiceTests
    {
        private Mock<ILog> _loggerMock;
        private Mock<ISubscriptionService> _subscriptionMock;

        private InitialiseDeliveryService _sut;

        public InitialiseDeliveryServiceTests()
        {
        }

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();
            _subscriptionMock = new Mock<ISubscriptionService>();

            _sut = new InitialiseDeliveryService(_loggerMock.Object, _subscriptionMock.Object);
        }

        [Test]
        public void Given_I_Call_SetupSubscription_Then_Subscription_To_The_Data_Delivery_Queue_Is_Setup()
        {
            //act
            _sut.SetupSubscription();

            //assert
            _subscriptionMock.Verify(v => v.SubscribeToDataDeliveryQueue(), Times.Once);
        }

        [Test]
        public void Given_I_Call_CancelSubscription_Then_Subscription_To_The_Data_Delivery_Queue_Is_Cancelled()
        {
            //act
            _sut.CancelSubscription();

            //assert
            _subscriptionMock.Verify(v => v.CancelAllSubscriptionsAndDispose(), Times.Once);
        }
    }
}
