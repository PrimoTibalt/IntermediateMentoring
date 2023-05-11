using Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Cryptography;

var factory = new ConnectionFactory
{
    HostName = Configuration.HOST_NAME,
    UserName = Configuration.USER_NAME,
    Password = Configuration.PASSWORD,
};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.QueueDeclare(
    queue: Configuration.QUEUE_NAME,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = FileMessage.FromByteArray(body);
    var filePath = Path.Combine(Configuration.PATH_TO_SAVE, message.FileName);
    if (message.HashValue == null)
    {
        var dataLength = message.DataLength;
        var data = message.Data;
        var dataCRC32 = Crc32.Compute(data, 0, dataLength);
        if (dataCRC32 == message.CRC32)
        {
            using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fileStream.Seek(message.Offset, SeekOrigin.Begin);
                fileStream.Write(data, 0, dataLength);
            }
        }
        else
        {
            Console.WriteLine($"Received corrupted chunk {message.ChunkCounter} for file {filePath}");
        }
    }
    else
    {
        var hashValue = message.HashValue;
        using var fileStream = File.OpenRead(filePath);
        using var hashAlgorithm = SHA256.Create();
        var fileHash = hashAlgorithm.ComputeHash(fileStream);
        if (CompareHashes(fileHash, hashValue))
        {
            Console.WriteLine("Received file {0} with correct hash value", filePath);
            channel.BasicAck(ea.DeliveryTag, true);
        }
        else
        {
            Console.WriteLine("Received file {0} with incorrect hash value", filePath);
        }
    }
};

channel.BasicConsume(Configuration.QUEUE_NAME, false, consumer);
Console.ReadLine();
channel.Close();
connection.Close();

static bool CompareHashes(byte[] hash1, byte[] hash2)
{
    if (hash1.Length != hash2.Length)
        return false;

    for (var i = 0; i < hash1.Length; i++)
    {
        if (hash1[i] != hash2[i])
            return false;
    }

    return true;
}