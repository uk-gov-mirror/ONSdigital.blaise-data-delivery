using BlaiseDataDelivery.Interfaces.Services.File;
using BlaiseDataDelivery.Services.File;
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

        private FileService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _fileSystemServiceMock = new Mock<IFileSystemService>();
            _fileSystemServiceMock.Setup(f => f.MoveFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _encryptionServiceMock = new Mock<IFileEncryptionService>();
            _zipServiceMock = new Mock<IFileZipService>();

            _sut = new FileService(_fileSystemServiceMock.Object, _encryptionServiceMock.Object, _zipServiceMock.Object);
        }


        [Test]
        public void Given_Valid_Arguments_When_I_Call_MoveFilesToSubFolder_Then_The_TemporaryPath_Is_returned()
        {
            //arrange
            var sourceFilePath = "sourcePath";
            var filePattern = "*.b*";
            var subFolderName = "temp";

            var expectedTemporaryPath = $"{sourceFilePath}\\{subFolderName}";

            //act
            var result = _sut.MoveFilesToSubFolder(sourceFilePath, filePattern, subFolderName);

            //assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual(expectedTemporaryPath, result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_MoveFilesToSubFolder_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var sourceFilePath = "sourcePath";
            var filePattern = "*.b*";
            var subFolderName = "temp";

            //act
            _sut.MoveFilesToSubFolder(sourceFilePath, filePattern, subFolderName);

            //assert
            _fileSystemServiceMock.Verify(v => v.MoveFiles(sourceFilePath, It.IsAny<string>(), filePattern), Times.Once);
        }

        [TestCase("", "*.b*", "temp", "A value for the argument 'sourceFilePath' must be supplied")]
        [TestCase("sourcePath", "", "temp", "A value for the argument 'filePattern' must be supplied")]
        [TestCase("sourcePath", "*.b*", "", "A value for the argument 'subFolderName' must be supplied")]
        public void Given_An_Empty_Argument_When_I_Call_MoveFilesToSubFolder_Then_An_ArgumentException_Is_Thrown(string sourceFilePath, string filePattern, string subFolderName, string errorMessage)
        {

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MoveFilesToSubFolder(sourceFilePath, filePattern, subFolderName));
            Assert.AreEqual(errorMessage, result.Message);
        }

        [TestCase(null, "*.b*", "temp", "sourceFilePath")]
        [TestCase("sourcePath", null, "temp", "filePattern")]
        [TestCase("sourcePath", "*.b*", null, "subFolderName")]
        public void Given_A_Null_Argument_When_I_Call_MoveFilesToSubFolder_Then_An_ArgumentException_Is_Thrown(string sourceFilePath, string filePattern, string subFolderName, string errorMessage)
        {

            //act && assert
            var result = Assert.Throws<ArgumentNullException>(() => _sut.MoveFilesToSubFolder(sourceFilePath, filePattern, subFolderName));
            Assert.AreEqual(errorMessage, result.ParamName);
        }
    }
}
