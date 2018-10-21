using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.Analyzers
{
    public class AzureServiceBusNotificatorSettings
    {
        public string ConnectionString { get; set; }
        public string AnalyzeNotificatorQueueName { get; set; }
    }
}
