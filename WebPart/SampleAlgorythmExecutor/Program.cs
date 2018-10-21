using AnalyzeProtocol;
using DicomParser;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleAlgorythmExecutor.InputMetas;
using System;
using System.Diagnostics;
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
        private static Core core;
        private static string cppExePath;

        private static async Task Main(string[] args)
        {
            var fileContent = await File.ReadAllTextAsync("appsettings.Secret.json");
            var json = JsonConvert.DeserializeObject<JObject>(fileContent);
            var connectionString = json["ConnectionString"].ToString();
            var queueName = json["AnalyzeNotificatorQueueName"].ToString();
            var coreHost = json["CoreHost"].ToString();
            cppExePath = json["CppExePath"].ToString();

            queueClient = new QueueClient(connectionString, queueName);
            core = new Core(coreHost);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxAutoRenewDuration = TimeSpan.FromMinutes(40),
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(TryProcessMessagesAsync, messageHandlerOptions);
            Console.ReadKey();
        }
        static async Task TryProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                await ProcessMessagesAsync(message, token);
            }
            catch
            {
                await queueClient.AbandonAsync(message.SystemProperties.LockToken);
                throw;
            }
        }
        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var body = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{body}");
           
            var analyzeMessage = JsonConvert.DeserializeObject<AnalyzeMessage>(body);
            await core.UpdateStatus(analyzeMessage.AnalyzeId);
            var dicomFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await core.DownloadToFolder(analyzeMessage.RecordId, dicomFolder);
            var picsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(picsFolder);
            var picturePathesTasks = Directory
                .EnumerateFiles(dicomFolder)
                .Select<string, Task<(Stream picStream, double deep)>>(async (p, i) =>
                {
                    var bitMap = await DicomRenderer.Render(File.OpenRead(p), out var localMeta);
                    var deep = double.TryParse(localMeta.FirstOrDefault(m => m.Description?.Contains("Slice Location") == true).Value, out var calcDeep)
                        ? calcDeep : 0;
                    return (bitMap, deep);
                    
                })
                .ToList();
            await Task.WhenAll(picturePathesTasks);
            var results = picturePathesTasks
                .Select(t => t.Result)
                .OrderBy(t => t.deep)
                .Select(async (t, i) =>
                {
                    var picPath = Path.Combine(picsFolder, $"{i}.png");
                    using (var picStream = File.OpenWrite(picPath))
                        await t.picStream.CopyToAsync(picStream);
                    return picPath;
                })
                .ToList();
            await Task.WhenAll(results);

            var targetDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetDir);
            var meta = new InputMeta
            {
                Photos = results.Select(p => new PhotoMeta { Path = p.Result}).ToList(),
                TargetDir = targetDir
            };
            var metaFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".json");
            File.WriteAllText(metaFilePath, JsonConvert.SerializeObject(meta));
            var proccess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cppExePath,
                    Arguments = metaFilePath
                }
            };
            proccess.Start();
            proccess.WaitForExit();
            Console.WriteLine(string.Join(',', Directory.GetFiles(targetDir)));


            await core.SendAnswer(analyzeMessage.AnalyzeId, Directory.EnumerateFiles(targetDir).Select(File.OpenRead));
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
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
