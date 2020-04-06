using BlaiseDataDelivery.Helpers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileEncryptionService _encryptionService;
        private readonly IFileZipService _zipService;

        public FileProcessingService(
            IFileSystemService fileSystemService,
            IFileEncryptionService encryptionService,
            IFileZipService zipService)
        {
            _fileSystemService = fileSystemService;
            _encryptionService = encryptionService;
            _zipService = zipService;
        }

        public void CreateZipFile(IEnumerable<string> files, string path)
        {
            _zipService.CreateZipFile(files, path);
        }

        public void DeleteFiles(IEnumerable<string> files)
        {
            throw new NotImplementedException();
        }

        public void EncryptFiles(IEnumerable<string> files)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles(string path, string filePattern)
        {
            path.ThrowExceptionIfNullOrEmpty("path");
            filePattern.ThrowExceptionIfNullOrEmpty("filePattern");

            return _fileSystemService.GetFiles(path, filePattern);
        }
    }
}
