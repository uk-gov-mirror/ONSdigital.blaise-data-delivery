
using BlaiseDataDelivery.Interfaces.Services.Files;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace BlaiseDataDelivery.Services.Files
{
    public class FileEncryptionService : IFileEncryptionService
    {
        private const string PublicKeyName = "RsaKey.pub";

        public void EncryptFile(string inputFilePath, string outputFilePath)
        {
            var publicKeyPath = Path.GetFullPath(PublicKeyName);
            var publicKey = ReadPublicKey(publicKeyPath);

            EncryptFile(inputFilePath, outputFilePath, publicKey);
        }

        private void EncryptFile(string inputFilePath, string outputFilePath, PgpPublicKey publicKey)
        {
            using (var fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                EncryptFile(streamWriter.BaseStream, inputFilePath, publicKey);
            }
        }

        private void EncryptFile(Stream outputStream, string filePath, PgpPublicKey publicKey)
        {
            using (MemoryStream outputMemoryStream = new MemoryStream())
            {
                PgpCompressedDataGenerator compressedData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                PgpUtilities.WriteFileToLiteralData(compressedData.Open(outputMemoryStream), PgpLiteralData.Binary, new FileInfo(filePath));
                compressedData.Close();

                var bytes = outputMemoryStream.ToArray();

                var encryptedData = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, true, new SecureRandom());
                encryptedData.AddMethod(publicKey);

                var encryptedStream = encryptedData.Open(outputStream, bytes.Length);
                encryptedStream.Write(bytes, 0, bytes.Length);
                encryptedStream.Close();
            }
        }

        private PgpPublicKey ReadPublicKey(string publicKeyFilePath)
        {
            Stream keyFileStream = File.OpenRead(publicKeyFilePath);
            keyFileStream = PgpUtilities.GetDecoderStream(keyFileStream);

            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(keyFileStream);

            foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            foreach (PgpPublicKey key in keyRing.GetPublicKeys())
            {
                if (key.IsEncryptionKey)
                {
                    keyFileStream.Close();
                    return key;
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring of the public key held at '{publicKeyFilePath}'");
        }
    }
}
