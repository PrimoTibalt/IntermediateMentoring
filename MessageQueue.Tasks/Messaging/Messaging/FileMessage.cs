using System.Runtime.Serialization.Formatters.Binary;

namespace Messaging
{
    [Serializable]
    public class FileMessage
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public long Offset { get; set; }
        public uint CRC32 { get; set; }
        public int DataLength { get; set; }
        public int ChunkCounter { get; set; }
        public byte[] Data { get; set; }
        public byte[] HashValue { get; set; }

        public byte[] ToByteArray()
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static FileMessage FromByteArray(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter();
                return (FileMessage)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}

