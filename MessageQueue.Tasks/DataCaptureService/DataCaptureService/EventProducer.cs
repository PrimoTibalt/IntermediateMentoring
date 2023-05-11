using RabbitMQ.Client;

namespace DataCaptureService
{
    static class FileEventProducer
    {
        public static FileSystemEventHandler GetSendingFileEvent(IModel channel)
        {
            return (object source, FileSystemEventArgs e) =>
            {
                try
                {
                    Console.WriteLine($"File: {e.Name} entered.");
                    while (!File.Exists(e.FullPath))
                    {
                        Console.WriteLine("File doesn't exist in file system");
                        Thread.Sleep(25);
                    }

                    Transferer.TransferFile(e.FullPath, channel);
                    Console.WriteLine($"File: {e.Name} sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed everything: " + ex.Message);
                }
            };
        }
    }
}
