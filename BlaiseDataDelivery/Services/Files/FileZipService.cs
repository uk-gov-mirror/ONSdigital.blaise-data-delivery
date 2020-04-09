
using BlaiseDataDelivery.Helpers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileZipService : IFileZipService
    {
        public void CreateZipFile(IEnumerable<string> files, string filePath)
        {
            if (!files.Any())
            {
                throw new ArgumentException($"No files provided");
            }

            filePath.ThrowExceptionIfNullOrEmpty("filePath");

            //create any folders that may not exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFiles(files, @"\");
                zip.Save(streamWriter.BaseStream);
            }
        }
    }
}
