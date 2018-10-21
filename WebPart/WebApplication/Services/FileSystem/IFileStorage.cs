using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.FileSystem
{
    public interface IFileStorage
    {
        Task SaveMRData(Guid id, IEnumerable<Stream> dcioFiles);
        Task<IEnumerable<Stream>> GetMRData(Guid id);

        Task SaveAnalyzeResult(Guid id, IEnumerable<Stream> dcioFiles);
        Task<IEnumerable<Stream>> GetAnalyzeResult(Guid id);
        Task<Stream> GetAnalyzeResult(Guid id, int num);
    }
}
