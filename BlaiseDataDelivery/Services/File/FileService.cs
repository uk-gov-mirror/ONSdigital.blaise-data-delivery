using BlaiseDataDelivery.Helpers;
using BlaiseDataDelivery.Interfaces.Services.File;
using System;

namespace BlaiseDataDelivery.Services.File
{
    public class FileService : IFileService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileEncryptionService _encryptionService;
        private readonly IFileZipService _zipService;

        public FileService(
            IFileSystemService fileSystemService,
            IFileEncryptionService encryptionService,
            IFileZipService zipService)
        {
            _fileSystemService = fileSystemService;
            _encryptionService = encryptionService;
            _zipService = zipService;
        }

        public string CreateZipFile(string sourceFilePath)
        {
            throw new NotImplementedException();
        }

        public void DeleteTemporaryFiles(string sourceFilePath)
        {
            throw new NotImplementedException();
        }

        public void DeployZipFile(string sourceFilePath, string destinationFilePath)
        {
            throw new NotImplementedException();
        }

        public string EncryptFiles(string sourceFilePath)
        {
            throw new NotImplementedException();
        }

        public string MoveFilesToSubFolder(string sourceFilePath, string filePattern, string subFolderName)
        {
            sourceFilePath.ThrowExceptionIfNullOrEmpty("sourceFilePath");
            filePattern.ThrowExceptionIfNullOrEmpty("filePattern");
            subFolderName.ThrowExceptionIfNullOrEmpty("subFolderName");

            var destinationFilePath = $"{sourceFilePath}\\{subFolderName}";

            _fileSystemService.MoveFiles(sourceFilePath, destinationFilePath, filePattern);

            return destinationFilePath;
        }

        public void RestoreFilesToOriginalLocation(string sourceFilePath, string destinationFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
