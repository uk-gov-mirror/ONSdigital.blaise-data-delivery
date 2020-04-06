using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.MessageHandlers;
using BlaiseDataDelivery.Models;
using log4net;
using Moq;
using NUnit.Framework;

namespace BlaiseDataDelivery.Tests.MessageHandler
{
    public class DataDeliveryMessageHandlerTests
    {
        private Mock<ILog> _loggerMock;
        private Mock<IConfigurationProvider> _configurationMock;
        private Mock<IMessageModelMapper> _mapperMock;
        private Mock<IFileProcessingService> _fileServiceMock;

        private readonly string _messageType;
        private readonly string _message;
        private readonly MessageModel _messageModel;

        private DataDeliveryMessageHandler _sut;

        public DataDeliveryMessageHandlerTests()
        {
            _messageType = "MessageType";
            _message = "Message";

            _messageModel = new MessageModel
            {
                SourceFilePath = "SourcePath",
                OutputFilePath = "OutputPath"
            };
        }

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();

            _configurationMock = new Mock<IConfigurationProvider>();

            _mapperMock = new Mock<IMessageModelMapper>();
            _mapperMock.Setup(m => m.MapToMessageModel(_message)).Returns(_messageModel);

            _fileServiceMock = new Mock<IFileProcessingService>();
            _fileServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _fileServiceMock.Setup(f => f.EncryptFiles(It.IsAny<string>())).Returns(It.IsAny<string>());
            _fileServiceMock.Setup(f => f.CreateZipFile(It.IsAny<string>())).Returns(It.IsAny<string>());
            _fileServiceMock.Setup(f => f.DeployZipFile(It.IsAny<string>(), It.IsAny<string>()));
            _fileServiceMock.Setup(f => f.DeleteTemporaryFiles(It.IsAny<string>()));


            _sut = new DataDeliveryMessageHandler(
                _loggerMock.Object, _configurationMock.Object, _mapperMock.Object, _fileServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_HandleMessage_Is_Handled_Correctly_Then_True_Is_Returned()
        {
            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            Assert.IsNotNull(result);
            Assert.True(result);
        }

        [Test]
        public void Given_Valid_Arguments_When_HandleMessage_Is_Called_Then_Correct_Services_Are_Called()
        {
            //arrange
            var _filePattern = "*.*";
            var _temporaryPath = string.Empty;
            var _encryptedPath = "EncryptedPath";
            var _zipPath = "ZipPath";

            _configurationMock.Setup(c => c.FilePattern).Returns(_filePattern);

            _fileServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((source, dest, pat) => _temporaryPath = dest);

            _fileServiceMock.Setup(f => f.EncryptFiles(It.IsAny<string>())).Returns(_encryptedPath);
            _fileServiceMock.Setup(f => f.CreateZipFile(It.IsAny<string>())).Returns(_zipPath);

            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            _mapperMock.Verify(v => v.MapToMessageModel(_message), Times.Once);
            _fileServiceMock.Verify(v => v.MoveFiles(_messageModel.SourceFilePath, It.IsAny<string>(), _filePattern), Times.Once);
            _fileServiceMock.Verify(v => v.EncryptFiles(_temporaryPath), Times.Once);
            _fileServiceMock.Verify(v => v.CreateZipFile(_encryptedPath), Times.Once);
            _fileServiceMock.Verify(v => v.DeployZipFile(_zipPath, _messageModel.OutputFilePath), Times.Once);
            _fileServiceMock.Verify(v => v.DeleteTemporaryFiles(_temporaryPath), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_HandleMessage_Is_Called_Then_A_Unique_SubFolder_Is_Created_each_Time()
        {
            //srrange act && assert
            var tempPath1 = string.Empty;
            _fileServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((source, dest, pat) => tempPath1 = dest);
            _sut.HandleMessage(_messageType, _message);


            var tempPath2 = string.Empty;
            _fileServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((source, dest, pat) => tempPath2 = dest);
            _sut.HandleMessage(_messageType, _message);

            Assert.AreNotEqual(tempPath1, tempPath2);
        }
    }
}
