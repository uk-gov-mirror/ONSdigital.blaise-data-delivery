using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileProcessingService
    {
        void CreateZipFile(IEnumerable<string> files, string filePath);
        void DeleteFiles(IEnumerable<string> files);
        void EncryptFiles(IEnumerable<string> files);
        IEnumerable<string> GetFiles(string path, string filePattern);
        void DeployFileToBucket(string zipFilePath);
    }
}
