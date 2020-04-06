using BlaiseDataDelivery.Interfaces.Services.Files;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IFileDirectoryService _directoryService;
        private readonly IFileEncryptionService _encryptionService;
        private readonly IFileZipService _zipService;
        private readonly IFileCloudStorageService _cloudStorageService;

        public FileService(
            IFileDirectoryService directoryService,
            IFileEncryptionService encryptionService,
            IFileZipService zipService,
            IFileCloudStorageService cloudStorageService)
        {
            _directoryService = directoryService;
            _encryptionService = encryptionService;
            _zipService = zipService;
            _cloudStorageService = cloudStorageService;
        }

        public void CreateZipFile(IEnumerable<string> files, string filePath)
        {
            _zipService.CreateZipFile(files, filePath);
        }

        public void DeleteFile(string filePath)
        {
            _directoryService.DeleteFile(filePath);
        }

        public void DeleteFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                _directoryService.DeleteFile(file);
            }
        }
        public void EncryptFiles(IEnumerable<string> files)
        {
            return;
        }

        public IEnumerable<string> GetFiles(string path, string filePattern)
        {
            return _directoryService.GetFiles(path, filePattern);
        }

        public void UploadFileToBucket(string zipFilePath, string bucketName)
        {
            _cloudStorageService.UploadFileToBucket(zipFilePath, bucketName);
        }
    }
}
