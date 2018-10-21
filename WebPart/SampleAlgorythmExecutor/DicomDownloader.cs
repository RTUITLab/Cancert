using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleAlgorythmExecutor
{
    class DicomDownloader
    {
        private readonly HttpClient client;

        public DicomDownloader(string hostName)
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
    }
}
