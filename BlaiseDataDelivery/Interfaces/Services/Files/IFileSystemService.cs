using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetFiles(string path, string filePattern);

        void MoveFiles(IEnumerable<string> files, string destinationPath);
    }
}
