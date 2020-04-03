

namespace BlaiseDataDelivery.Interfaces.Services.File
{
    public interface IFileService
    {
        /// <summary>
        /// Moved files to a sub golder
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="filePattern"></param>
        /// <param name="subFolderName"></param>
        /// <returns>Full path to the sub folder</returns>
        string MoveFilesToSubFolder(string sourceFilePath, string filePattern, string subFolderName);
        string EncryptFiles(string sourceFilePath);
        string CreateZipFile(string sourceFilePath);
        void DeployZipFile(string sourceFilePath, string destinationFilePath);
        void DeleteTemporaryFiles(string sourceFilePath);
        void RestoreFilesToOriginalLocation(string sourceFilePath, string destinationFilePath);
    }
}
