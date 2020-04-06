using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileProcessingService
    {
        void CreateZipFile(IEnumerable<string> files, string path);
        void DeleteFiles(IEnumerable<string> files);
        void EncryptFiles(IEnumerable<string> files);
        IEnumerable<string> GetFiles(string path, string filePattern);
    }
}
