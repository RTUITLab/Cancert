using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.FileSystem
{
    public class LocalFileSystemFileStorage : IFileStorage
    {
        private LocalFileSystemSettings settings;

        public LocalFileSystemFileStorage(IOptions<LocalFileSystemSettings> options)
        {
            settings = options.Value;
        }
        public Task<IEnumerable<Stream>> GetMRData(Guid id)
        {
            var targetDir = Path.Combine(settings.WorkDirectory, id.ToString());
            if (!Directory.Exists(targetDir))
            {
                return Task.FromResult<IEnumerable<Stream>>(new Stream[0]);
            }
            return Task.FromResult<IEnumerable<Stream>>(
                Directory
                .GetFiles(targetDir)
                .Select(File.OpenRead));
        }

        public async Task SaveMRData(Guid id, IEnumerable<Stream> dcioFiles)
        {
            var targetDir = Path.Combine(settings.WorkDirectory, id.ToString());
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, recursive: true);
            }
            Directory.CreateDirectory(targetDir);
            var tasks = dcioFiles.Select((s, i) => Task.Run(async () =>
            {
                using (var fileStream = File.OpenWrite(Path.Combine(targetDir, $"{i}.dcm")))
                    await s.CopyToAsync(fileStream);
            })).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}
