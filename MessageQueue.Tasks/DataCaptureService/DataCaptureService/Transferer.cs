using Messaging;
using RabbitMQ.Client;
using System.Security.Cryptography;

namespace DataCaptureService
{
    static class Transferer
    {
        public const int CHUNK_SIZE = 1200000;

        public static void TransferFile(string filePath, IModel channel)
        {
            using (var fileStream = GetFileStream(filePath))
            {
                var fileName = Path.GetFileName(filePath);
                var fileSize = fileStream.Length;
                var buffer = new byte[CHUNK_SIZE];
                var chunkCounter = 0;
                using (var hashAlgorithm = SHA256.Create())
                {
                    while (fileStream.Position < fileStream.Length)
                    {
                        var chunkBytes = fileStream.Read(buffer, 0, buffer.Length);
                        hashAlgorithm.TransformBlock(buffer, 0, chunkBytes, null, 0);
                        var message = new FileMessage
                        {
                            FileName = fileName,
                            FileSize = fileSize,
                            Offset = fileStream.Position - chunkBytes,
                            CRC32 = Crc32.Compute(buffer, 0, chunkBytes),
                            DataLength = chunkBytes,
                            ChunkCounter = ++chunkCounter,
                            Data = buffer
                        };
                        var body = message.ToByteArray();
                        channel.BasicPublish(exchange: "",
                            routingKey: Configuration.QUEUE_NAME,
                            basicProperties: null,
                            body: body);
                    }

                    hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
                    var finalMessage = new FileMessage
                    {
                        FileName = fileName,
                        HashValue = hashAlgorithm.Hash
                    };
                    var finalBody = finalMessage.ToByteArray();
                    channel.BasicPublish(exchange: "",
                        routingKey: Configuration.QUEUE_NAME,
                        basicProperties: null,
                        body: finalBody);
                }
            }
        }

        private static FileStream GetFileStream(string filePath, int counter = 0)
        {
            if (counter > 10)
                throw new ArgumentException("File isn't accessible");

            try
            {
                return File.OpenRead(filePath);
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                return GetFileStream(filePath, ++counter);
            }
        }
    }
}
