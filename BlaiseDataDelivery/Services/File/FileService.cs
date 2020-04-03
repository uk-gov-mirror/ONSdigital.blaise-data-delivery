using BlaiseDataDelivery.Interfaces.Services.File;

namespace BlaiseDataDelivery.Services.File
{
    public class FileService : IFileService
    {
        public string CreateZipFile(string sourceFilePath)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTemporaryFiles(string sourceFilePath)
        {
            throw new System.NotImplementedException();
        }

        public void DeployZipFile(string sourceFilePath, string destinationFilePath)
        {
            throw new System.NotImplementedException();
        }

        public string EncryptFiles(string sourceFilePath)
        {
            throw new System.NotImplementedException();
        }

        public string MoveFilesToTemporaryLocation(string sourceFilePath)
        {
            throw new System.NotImplementedException();
        }

        public void RestoreFilesToOriginalLocation(string sourceFilePath, string destinationFilePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
