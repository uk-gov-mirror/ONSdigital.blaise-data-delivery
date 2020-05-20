
using System.Collections.Generic;

namespace BlaiseDataDelivery.Interfaces.Services.Files
{
    public interface IFileZipService
    {
        void CreateZipFile(IList<string> files, string filePath);
    }
}
