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
        private Mock<IQueueService> _subscriptionMock;

        private InitialiseService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();
            _subscriptionMock = new Mock<IQueueService>();

            _sut = new InitialiseService(_loggerMock.Object, _subscriptionMock.Object);
        }

        [Test]
        public void Given_I_Call_SetupSubscription_Then_Subscription_To_The_Data_Delivery_Queue_Is_Setup()
        {
            //act
            _sut.Start();

            //assert
            _subscriptionMock.Verify(v => v.Subscribe(), Times.Once);
        }

        [Test]
        public void Given_I_Call_CancelSubscription_Then_Subscription_To_The_Data_Delivery_Queue_Is_Cancelled()
        {
            //act
            _sut.Stop();

            //assert
            _subscriptionMock.Verify(v => v.CancelAllSubscriptions(), Times.Once);
        }
    }
}
