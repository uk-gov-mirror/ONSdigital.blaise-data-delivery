using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.MessageHandlers;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.MessageHandler
{
    public class DataDeliveryMessageHandlerTests
    {
        private Mock<IMessageModelMapper> _mapper;

        private DataDeliveryMessageHandler _sut;

        [SetUp]
        public void SetUpTests()
        {
            _mapper = new Mock<IMessageModelMapper>();

            _sut = new DataDeliveryMessageHandler(_mapper.Object);
        }
    }
}
