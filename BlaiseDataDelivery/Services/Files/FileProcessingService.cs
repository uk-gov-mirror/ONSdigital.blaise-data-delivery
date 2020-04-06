using BlaiseDataDelivery.Interfaces.Services.Files;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileDirectoryService _directoryService;
        private readonly IFileEncryptionService _encryptionService;
        private readonly IFileZipService _zipService;

        public FileProcessingService(
            IFileDirectoryService directoryService,
            IFileEncryptionService encryptionService,
            IFileZipService zipService)
        {
            _directoryService = directoryService;
            _encryptionService = encryptionService;
            _zipService = zipService;
        }

        public void CreateZipFile(IEnumerable<string> files, string filePath)
        {
            _zipService.CreateZipFile(files, filePath);
        }

        public void DeleteFiles(IEnumerable<string> files)
        {
            return;
        }

        public void DeployFileToBucket(string zipFilePath)
        {
            return;
        }

        public void EncryptFiles(IEnumerable<string> files)
        {
            return;
        }

        public IEnumerable<string> GetFiles(string path, string filePattern)
        {
            return _directoryService.GetFiles(path, filePattern);
        }
    }
}
