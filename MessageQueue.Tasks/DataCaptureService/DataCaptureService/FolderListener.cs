using RabbitMQ.Client;

namespace DataCaptureService
{
    static class FolderListener
    {
        private static readonly string[] filters = { "*.xml", "*.pdf" };

        public static IList<FileSystemWatcher> Listen(string folderPath, IModel channel)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            var watchers = new List<FileSystemWatcher>();
            foreach (var filter in filters)
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = folderPath;
                watcher.Filter = filter;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                var onFileCreated = FileEventProducer.GetSendingFileEvent(channel);
                watcher.Created += onFileCreated;

                watcher.EnableRaisingEvents = true;
                watchers.Add(watcher);
            }

            return watchers;
        }
    }
}
