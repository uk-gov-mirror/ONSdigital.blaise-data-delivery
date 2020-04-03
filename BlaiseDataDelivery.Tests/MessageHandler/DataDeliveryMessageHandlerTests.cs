using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Services.File;
using BlaiseDataDelivery.MessageHandlers;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.MessageHandler
{
    public class DataDeliveryMessageHandlerTests
    {
        private Mock<ILog> _loggerMock;
        private Mock<IMessageModelMapper> _mapperMock;
        private Mock<IFileService> _fileServiceMock;

        private DataDeliveryMessageHandler _sut;

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();
            _mapperMock = new Mock<IMessageModelMapper>();
            _fileServiceMock = new Mock<IFileService>();

            _sut = new DataDeliveryMessageHandler(_loggerMock.Object, _mapperMock.Object, _fileServiceMock.Object);
        }
    }
}
