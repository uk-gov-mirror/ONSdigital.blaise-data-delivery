
namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileCloudStorageService
    {
        void UploadFileToBucket(string filePath, string bucketName);
    }
}
