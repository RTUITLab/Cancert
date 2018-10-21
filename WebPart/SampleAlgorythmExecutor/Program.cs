using AnalyzeProtocol;
using DicomParser;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleAlgorythmExecutor.InputMetas;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleAlgorythmExecutor
{
    class Program
    {
        private static QueueClient queueClient;
        private static DicomDownloader dicomDownloader;

        private static async Task Main(string[] args)
        {
            var fileContent = await File.ReadAllTextAsync("appsettings.Secret.json");
            var json = JsonConvert.DeserializeObject<JObject>(fileContent);
            var connectionString = json["ConnectionString"].ToString();
            var queueName = json["AnalyzeNotificatorQueueName"].ToString();
            var coreHost = json["CoreHost"].ToString();


            queueClient = new QueueClient(connectionString, queueName);
            dicomDownloader = new DicomDownloader(coreHost);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxAutoRenewDuration = TimeSpan.FromSeconds(40),
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            Console.ReadKey();
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{body}");
            var analyzeMessage = JsonConvert.DeserializeObject<AnalyzeMessage>(body);
            var dicomFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await dicomDownloader.DownloadToFolder(analyzeMessage.RecordId, dicomFolder);
            var picsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(picsFolder);
            var picturePathesTasks = Directory
                .EnumerateFiles(dicomFolder)
                .Select(async (p, i) =>
                {
                    var picPath = Path.Combine(picsFolder, $"{i}.png");
                    var bitMap = await DicomRenderer.Render(File.OpenRead(p));
                    using (var picStream = File.OpenWrite(picPath))
                        await bitMap.CopyToAsync(picStream);
                    return picPath;
                })
                .ToList();
            await Task.WhenAll(picturePathesTasks);
            var targetDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var meta = new InputMeta
            {
                Photos = picturePathesTasks.Select(p => new PhotoMeta { Path = p.Result }).ToList(),
                TargetDir = targetDir
            };
            var metaFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".json");
            File.WriteAllText(metaFilePath, JsonConvert.SerializeObject(meta));
        }


        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
