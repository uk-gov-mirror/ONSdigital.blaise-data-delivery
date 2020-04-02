
using System;

namespace BlaiseDataDelivery.Interfaces.Services.File
{
    public interface IFileService
    {
        string MoveFilesToTemporaryLocation(string sourceFilePath);
        string EncryptFiles(string sourceFilePath);
        string CreateZipFile(string sourceFilePath);
        void DeployZipFile(string sourceFilePath, string destinationFilePath);
        void DeleteTemporaryFiles(string sourceFilePath);
        void RestoreFilesToOriginalLocation(string sourceFilePath, string destinationFilePath);
    }
}
