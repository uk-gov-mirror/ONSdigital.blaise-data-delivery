using BlaiseDataDelivery.Interfaces.Services.Files;
using System.IO;
using System.Linq;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileSystemService : IFileSystemService
    {
        public void MoveFiles(string sourceFilePath, string destinationFilePath, string filePattern)
        {
            var sourceFolder = new DirectoryInfo(sourceFilePath);

            if(!sourceFolder.Exists)
            {
                throw new DirectoryNotFoundException($"The dirctory '{sourceFilePath}' was not found");
            }

            var filesToMove = sourceFolder.GetFiles(filePattern);

            if(!filesToMove.Any())
            {
                throw new FileNotFoundException($"No files matching the file pattern '{filePattern}' were found");
            }

            foreach(var fileToMove in filesToMove)
            {
                File.Move(fileToMove.FullName, destinationFilePath);
            }
        }
    }
}
