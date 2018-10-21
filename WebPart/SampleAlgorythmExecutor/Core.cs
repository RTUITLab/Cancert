using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleAlgorythmExecutor
{
    class Core
    {
        private readonly HttpClient client;

        public Core(string hostName)
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(hostName)
            };
        }

        public async Task DownloadToFolder(Guid recordId, string folderPath)
        {
            var zipStream = await client.GetStreamAsync($"/api/mr/record/{recordId}/content");
            var zipfileName = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
            try
            {
                using (var fileStream = File.OpenWrite(zipfileName))
                    await zipStream.CopyToAsync(fileStream);
                ZipFile.ExtractToDirectory(zipfileName, folderPath);
            }
            finally
            {
                File.Delete(zipfileName);
            }
        }

        public async Task UpdateStatus(Guid analyzeId)
        {
            var result = await client.GetAsync($"/api/mr/analyze/startWork/{analyzeId}");
            Console.WriteLine($"Updated status: {result.StatusCode}");
        }

        public async Task SendAnswer(Guid analyzeId, IEnumerable<Stream> pics)
        {
            var content = new MultipartFormDataContent();
            int i = 0;
            foreach (var item in pics)
            {
                content.Add(new StreamContent(item), "pics", $"{i++}.png");
            }
            var response = await client.PostAsync($"/api/mr/analyze/accept/{analyzeId}", content);
            Console.WriteLine($"Sended results: {response.StatusCode}");
        }
    }
}
