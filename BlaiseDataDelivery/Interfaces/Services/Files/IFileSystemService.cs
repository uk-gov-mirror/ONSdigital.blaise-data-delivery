namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileSystemService
    {
        void MoveFiles(string sourceFilePath, string destinationFilePath, string filePattern);
    }
}
