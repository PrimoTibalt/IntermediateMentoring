using DataCaptureService;
using Messaging;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    HostName = Configuration.HOST_NAME,
    UserName = Configuration.USER_NAME,
    Password = Configuration.PASSWORD,
};
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
var watchers = FolderListener.Listen(Configuration.PATH_TO_LISTEN, channel);
Console.ReadLine();
foreach (var watcher in watchers)
    watcher.Dispose();
channel.Close();
connection.Close();
