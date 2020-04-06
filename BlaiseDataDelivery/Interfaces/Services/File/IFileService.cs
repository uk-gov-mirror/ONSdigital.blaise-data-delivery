

namespace BlaiseDataDelivery.Interfaces.Services.File
{
    public interface IFileService
    {
        void MoveFiles(string sourceFilePath, string destinationFilePath, string filePattern);
        string EncryptFiles(string sourceFilePath);
        string CreateZipFile(string sourceFilePath);
        void DeployZipFile(string sourceFilePath, string destinationFilePath);
        void DeleteTemporaryFiles(string sourceFilePath);
        void RestoreFilesToOriginalLocation(string sourceFilePath, string destinationFilePath);
    }
}
