using AnalyzeProtocol;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Models.Data;
using WebApplication.Services.FileSystem;

namespace WebApplication.Services.Analyzers
{
    public class AzureServiceBusNotificatorAnalyzer : IAnalyzer
    {
        private readonly QueueClient queueClient;
        private readonly IFileStorage fileStorage;

        public AzureServiceBusNotificatorAnalyzer(
            IOptions<AzureServiceBusNotificatorSettings> options,
            IFileStorage fileStorage)
        {
            queueClient = new QueueClient(options.Value.ConnectionString, options.Value.AnalyzeNotificatorQueueName);
            this.fileStorage = fileStorage;
        }
        public async Task Analyze(MrAnalyze mrAnalyze)
        {
            var message = new AnalyzeMessage
            {
                AlgorithmId = mrAnalyze.MrAlgorithmId,
                AnalyzeId = mrAnalyze.Id,
                RecordId = mrAnalyze.MrRecordId
            };
            var jsonMessage = JsonConvert.SerializeObject(message);
            var azureMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage));
            await queueClient.SendAsync(azureMessage);
        }
    }
}
