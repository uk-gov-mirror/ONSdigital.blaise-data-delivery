using BlaiseDataDelivery.Interfaces.Services.Queue;
using BlaiseDataDelivery.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlaiseDataDelivery.Tests.Services
{
    public class InitialiseDeliveryServiceTests
    {
        private Mock<ISubscriptionService> _subscriptionMock;

        private InitialiseDeliveryService _sut;

        public InitialiseDeliveryServiceTests()
        {
        }

        [SetUp]
        public void SetUpTests()
        {
            _subscriptionMock = new Mock<ISubscriptionService>();
        }

        [Test]
        public void Given_I_Call_SetupSubscription_Then_Subscription_Is_Setup()
        {
            //arrange

            //act

            //assert
        }
    }
}
