using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.Services.Files;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Tests.Services.Files
{
    public class FileProcessingServiceTests
    {
        private Mock<IFileDirectoryService> _fileSystemServiceMock;
        private Mock<IFileEncryptionService> _encryptionServiceMock;
        private Mock<IFileZipService> _zipServiceMock;

        private FileProcessingService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _fileSystemServiceMock = new Mock<IFileDirectoryService>();
            _fileSystemServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<IEnumerable<string>>);

            _encryptionServiceMock = new Mock<IFileEncryptionService>();
            _zipServiceMock = new Mock<IFileZipService>();

            _sut = new FileProcessingService(_fileSystemServiceMock.Object, _encryptionServiceMock.Object, _zipServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetFiles_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var path = "temp";
            var filePattern = "*.*";
            var files = new List<string> { "File1", "File2" };

            _fileSystemServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(files);

            //act
            var result = _sut.GetFiles(path, filePattern);

            //assert
            Assert.NotNull(result);
            Assert.AreEqual(files, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetFiles_Then_A_List_Of_Files_Are_Returned()
        {
            //arrange
            var path = "temp";
            var filePattern = "*.*";

            //act
            _sut.GetFiles(path, filePattern);

            //assert
            _fileSystemServiceMock.Verify(v => v.GetFiles(path, filePattern), Times.Once);
        }
    }
}
