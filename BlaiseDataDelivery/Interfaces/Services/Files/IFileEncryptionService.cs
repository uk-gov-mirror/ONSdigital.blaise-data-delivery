

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileEncryptionService
    {
        void EncryptFile(string inputFilePath, string outputFilePath);
    }
}
