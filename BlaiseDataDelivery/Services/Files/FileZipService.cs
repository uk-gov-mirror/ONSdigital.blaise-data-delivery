
using BlaiseDataDelivery.Interfaces.Services.Files;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileZipService : IFileZipService
    {
        public void CreateZipFile(IEnumerable<string> files, string path)
        {
            if (!files.Any())
            {
                throw new ArgumentException($"No files provided");
            }
        }
    }
}
