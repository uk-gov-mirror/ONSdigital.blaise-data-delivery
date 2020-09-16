using BlaiseDataDelivery.Models;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileService
    {
        string CreateEncryptedZipFile(IList<string> files, MessageModel messageModel);

        IEnumerable<string> GetFiles(string path, string messageModelInstrumentName, string filePattern);

        void UploadFileToBucket(string zipFilePath, string bucketName);
        void DeleteFiles(IEnumerable<string> files);
        void DeleteFile(string filePath);
    }
}
