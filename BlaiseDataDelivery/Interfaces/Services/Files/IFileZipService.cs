

using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileZipService
    {
        void CreateZipFile(IEnumerable<string> files, string filePath);
    }
}
