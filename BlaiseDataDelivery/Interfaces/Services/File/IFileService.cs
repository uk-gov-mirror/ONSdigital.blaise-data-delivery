
using System;

namespace BlaiseDataDelivery.Interfaces.Services.File
{
    public interface IFileService
    {
        void MoveFilesToTemporaryFolder(string sourceFilePath, string temporaryFilePath);
        string EncryptFiles(string temporaryFilePath);
        string CreateZipFile(string encryptedFilePath, string temporaryFilePath);
        void DeployZipFile(string zipFilePath, string outputFilePath);
        void DeleteTemporaryFiles(string temporaryFilePath);
        void RestoreFilesToOriginalLocation(string sourceFilePath, string temporaryFilePath);
    }
}
