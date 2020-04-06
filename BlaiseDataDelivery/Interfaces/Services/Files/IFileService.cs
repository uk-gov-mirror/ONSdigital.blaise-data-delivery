using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileService
    {
        void CreateZipFile(IEnumerable<string> files, string filePath);
        void EncryptFiles(IEnumerable<string> files);
        IEnumerable<string> GetFiles(string path, string filePattern);

        void UploadFileToBucket(string zipFilePath, string bucketName);
        void DeleteFiles(IEnumerable<string> filesToProcess);
        void DeleteFile(string zipFile);
    }
}
