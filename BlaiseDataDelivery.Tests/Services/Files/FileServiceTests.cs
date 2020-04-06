using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.Services.Files;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Tests.Services.Files
{
    public class FileServiceTests
    {
        private Mock<IFileDirectoryService> _fileSystemServiceMock;
        private Mock<IFileEncryptionService> _encryptionServiceMock;
        private Mock<IFileZipService> _zipServiceMock;
        private Mock<IFileCloudStorageService> _storageServiceMock;

        private FileService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _fileSystemServiceMock = new Mock<IFileDirectoryService>();
            _fileSystemServiceMock.Setup(f => f.GetFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<IEnumerable<string>>);

            _encryptionServiceMock = new Mock<IFileEncryptionService>();
            _zipServiceMock = new Mock<IFileZipService>();
            _storageServiceMock = new Mock<IFileCloudStorageService>();

            _sut = new FileService(
                _fileSystemServiceMock.Object, _encryptionServiceMock.Object, _zipServiceMock.Object, _storageServiceMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetFiles_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var path = "temp";
            var filePattern = "*.*";

            //act
            _sut.GetFiles(path, filePattern);

            //assert
            _fileSystemServiceMock.Verify(v => v.GetFiles(path, filePattern), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_GetFiles_Then_A_List_Of_Files_Are_Returned()
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
        public void Given_Valid_Arguments_When_I_Call_CreateZipFile_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var files = new List<string> { "File1", "File2" };
            var path = "temp";


            _zipServiceMock.Setup(f => f.CreateZipFile(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()));

            //act
            _sut.CreateZipFile(files, path);

            //assert
            _zipServiceMock.Verify(v => v.CreateZipFile(files, path), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UploadFileToBucket_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var file = "File1";
            var bucketName = "BucketName";


            _storageServiceMock.Setup(f => f.UploadFileToBucket(It.IsAny<string>(), It.IsAny<string>()));

            //act
            _sut.UploadFileToBucket(file, bucketName);

            //assert
            _storageServiceMock.Verify(v => v.UploadFileToBucket(file, bucketName), Times.Once);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_DeleteFiles_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var files = new List<string> { "File1", "File2" };

            _fileSystemServiceMock.Setup(f => f.DeleteFile(It.IsAny<string>()));

            //act
            _sut.DeleteFiles(files);

            //assert
            foreach (var file in files)
            {
                _fileSystemServiceMock.Verify(v => v.DeleteFile(file), Times.Once);
            }
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_DeleteFile_Then_The_Correct_Method_Is_Called()
        {
            //arrange
            var file = "File1";

            _fileSystemServiceMock.Setup(f => f.DeleteFile(It.IsAny<string>()));

            //act
            _sut.DeleteFile(file);

            //assert
            _fileSystemServiceMock.Verify(v => v.DeleteFile(file), Times.Once);
        }
    }
}
