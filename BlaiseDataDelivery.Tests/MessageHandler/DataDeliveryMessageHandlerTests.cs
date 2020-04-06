using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.MessageHandlers;
using BlaiseDataDelivery.Models;
using log4net;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Tests.MessageHandler
{
    public class DataDeliveryMessageHandlerTests
    {
        private Mock<ILog> _loggerMock;
        private Mock<IConfigurationProvider> _configurationMock;
        private Mock<IMessageModelMapper> _mapperMock;
        private Mock<IFileService> _fileServiceMock;

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
                OutputFilePath = "OutputPath",
                InstrumentName = "InstrumentName"
            };
        }

        [SetUp]
        public void SetUpTests()
        {
            _loggerMock = new Mock<ILog>();

            _configurationMock = new Mock<IConfigurationProvider>();

            _mapperMock = new Mock<IMessageModelMapper>();
            _mapperMock.Setup(m => m.MapToMessageModel(_message)).Returns(_messageModel);

            _fileServiceMock = new Mock<IFileService>();
            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<IEnumerable<string>>());
            _fileServiceMock.Setup(f => f.EncryptFiles(It.IsAny<IEnumerable<string>>()));
            _fileServiceMock.Setup(f => f.CreateZipFile(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()));
            _fileServiceMock.Setup(f => f.UploadFileToBucket(It.IsAny<string>(), It.IsAny<string>()));
            _fileServiceMock.Setup(f => f.DeleteFile(It.IsAny<string>()));
            _fileServiceMock.Setup(f => f.DeleteFiles(It.IsAny<IEnumerable<string>>()));

            _sut = new DataDeliveryMessageHandler(
                _loggerMock.Object, _configurationMock.Object, _mapperMock.Object, _fileServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_HandleMessage_Is_Called_Then_True_Is_Returned()
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
            var filePattern = "*.*";
            var bucketName = "BucketName";
            var filesToProcess = new List<string>
            {
                "File1",
                "File2"
            };

            var dateTime = DateTime.Now;
            var zipfilePath = $"SourcePath\\dd_InstrumentName_{dateTime:ddmmyy}_{dateTime:hhmmss}.zip";

            _configurationMock.Setup(c => c.FilePattern).Returns(filePattern);
            _configurationMock.Setup(c => c.BucketName).Returns(bucketName);

            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(filesToProcess);

            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            _mapperMock.Verify(v => v.MapToMessageModel(_message), Times.Once);

            _fileServiceMock.Verify(v => v.GetFiles(_messageModel.SourceFilePath, filePattern));
            _fileServiceMock.Verify(v => v.EncryptFiles(filesToProcess), Times.Once);
            _fileServiceMock.Verify(v => v.CreateZipFile(filesToProcess, zipfilePath), Times.Once);
            _fileServiceMock.Verify(v => v.UploadFileToBucket(zipfilePath, bucketName), Times.Once);
            _fileServiceMock.Verify(v => v.DeleteFile(zipfilePath), Times.Once);
            _fileServiceMock.Verify(v => v.DeleteFiles(filesToProcess), Times.Once);
        }

        [Test]
        public void Given_No_Files_Available_To_Process_When_HandleMessage_Is_Called_Then_True_Is_Returned()
        {
            //arrange
            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<string>());

            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            Assert.IsNotNull(result);
            Assert.True(result);
        }

        [Test]
        public void Given_No_Files_Available_To_Process_When_HandleMessage_Is_Called_Then_The_Services_Are_Not_Called()
        {
            //arrange
            var filePattern = "*.*";

            _configurationMock.Setup(c => c.FilePattern).Returns(filePattern);
            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<string>());

            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            _mapperMock.Verify(v => v.MapToMessageModel(_message), Times.Once);

            _fileServiceMock.Verify(v => v.GetFiles(_messageModel.SourceFilePath, filePattern));
            _fileServiceMock.Verify(v => v.EncryptFiles(It.IsAny<IEnumerable<string>>()), Times.Never);
            _fileServiceMock.Verify(v => v.CreateZipFile(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Never);
            _fileServiceMock.Verify(v => v.UploadFileToBucket(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _fileServiceMock.Verify(v => v.DeleteFile(It.IsAny<string>()), Times.Never);
            _fileServiceMock.Verify(v => v.DeleteFiles(It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [Test]
        public void Given_An_Exception_Occurs_During_Processing_When_HandleMessage_Is_Called_Then_The_Exception_Is_Not_Bubbled_Up()
        {
            //arrange
            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            //act
            var result = _sut.HandleMessage(_messageType, _message);

            //assert
            Assert.IsNotNull(result);
            Assert.True(result);
        }

        [Test]
        public void Given_An_Exception_Occurs_During_Processing_When_HandleMessage_Is_Called_Then_True_Is_Returned()
        {
            //arrange
            _fileServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            //act && assert
            Assert.DoesNotThrow(() => _sut.HandleMessage(_messageType, _message));
        }
    }
}
