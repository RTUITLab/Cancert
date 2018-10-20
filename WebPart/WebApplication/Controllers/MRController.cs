using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Services.Render;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MRController : ControllerBase
    {
        private readonly IRenderer renderer;

        public MRController(IRenderer renderer)
        {
            this.renderer = renderer;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(List<IFormFile> files)
        {
            if (files?.Count < 1)
                return BadRequest();
            var tasks = files.Select(f => renderer.Render(f.OpenReadStream())).ToArray();
            var folderName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var directoryInfo = Directory.CreateDirectory(folderName);
            var memoryZipStream = new MemoryStream();
            string zipFileName = "";
            try
            {
                await Task.WhenAll(tasks);
                foreach (var (picStream, FileName) in tasks.Zip(files, (task, f) => (task.Result, f.FileName)))
                {
                    using (var fileStream = System.IO.File.OpenWrite(Path.Combine(folderName, $"{FileName}.png")))
                    {
                        await picStream.CopyToAsync(fileStream);
                    }
                }
                zipFileName = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
                ZipFile.CreateFromDirectory(folderName, zipFileName);
                using (var fileZipStream = System.IO.File.OpenRead(zipFileName))
                {
                    await fileZipStream.CopyToAsync(memoryZipStream);
                    memoryZipStream.Position = 0;
                }
            }
            finally
            {
                directoryInfo.Delete(recursive: true);
                if (!string.IsNullOrEmpty(zipFileName))
                    System.IO.File.Delete(zipFileName);
            }
            return File(memoryZipStream, "application/zip", "dataset.zip");
        }
    }
}
