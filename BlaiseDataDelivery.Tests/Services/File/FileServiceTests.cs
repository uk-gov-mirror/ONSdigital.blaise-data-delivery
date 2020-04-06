using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.Services.Files;
using Moq;
using NUnit.Framework;
using System;

namespace BlaiseDataDelivery.Tests.Services.File
{
    public class FileServiceTests
    {
        private Mock<IFileSystemService> _fileSystemServiceMock;
        private Mock<IFileEncryptionService> _encryptionServiceMock;
        private Mock<IFileZipService> _zipServiceMock;

        private FileProcessingService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _fileSystemServiceMock = new Mock<IFileSystemService>();
            _fileSystemServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _encryptionServiceMock = new Mock<IFileEncryptionService>();
            _zipServiceMock = new Mock<IFileZipService>();

            _sut = new FileProcessingService(_fileSystemServiceMock.Object, _encryptionServiceMock.Object, _zipServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_MoveFilesToSubFolder_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var sourceFilePath = "sourcePath";
            var temporaryPath = "temp";
            var filePattern = "*.b*";

            //act
            _sut.MoveFiles(sourceFilePath, temporaryPath, filePattern);

            //assert
            _fileSystemServiceMock.Verify(v => v.MoveFiles(sourceFilePath, It.IsAny<string>(), filePattern), Times.Once);
        }

        [TestCase("", "destinationFilePath", "*.b*", "A value for the argument 'sourceFilePath' must be supplied")]
        [TestCase("sourceFilePath", "", "*.b*", "A value for the argument 'destinationFilePath' must be supplied")]
        [TestCase("sourceFilePath", "destinationFilePath", "", "A value for the argument 'filePattern' must be supplied")]
        public void Given_An_Empty_Argument_When_I_Call_MoveFilesToSubFolder_Then_An_ArgumentException_Is_Thrown(
            string sourceFilePath, string destinationFilePath, string filePattern, string errorMessage)
        {

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MoveFiles(sourceFilePath, destinationFilePath, filePattern));
            Assert.AreEqual(errorMessage, result.Message);
        }

        [TestCase(null, "destinationFilePath", "*.b*", "sourceFilePath")]
        [TestCase("sourcePath", null, "*.b*", "destinationFilePath")]
        [TestCase("sourcePath", "destinationFilePath", null, "filePattern")]
        public void Given_A_Null_Argument_When_I_Call_MoveFilesToSubFolder_Then_An_ArgumentException_Is_Thrown(string sourceFilePath, string destinationFilePath, string filePattern, string errorMessage)
        {

            //act && assert
            var result = Assert.Throws<ArgumentNullException>(() => _sut.MoveFiles(sourceFilePath, destinationFilePath, filePattern));
            Assert.AreEqual(errorMessage, result.ParamName);
        }
    }
}
