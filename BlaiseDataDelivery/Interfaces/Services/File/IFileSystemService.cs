namespace BlaiseDataDelivery.Interfaces.Services.File
{
    public interface IFileSystemService
    {
        void MoveFiles(string sourceFilePath, string destinationFilePath, string filePattern);
    }
}
