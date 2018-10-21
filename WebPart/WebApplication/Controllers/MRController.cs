using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicAPI.Requests;
using WebApplication.Controllers.Stuff;
using WebApplication.DataBase;
using WebApplication.Models.Data;
using WebApplication.Services.FileSystem;
using WebApplication.Services.Render;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MRController : HospitalController
    {
        private readonly IRenderer renderer;
        private readonly IFileStorage fileStorage;
        private readonly DataBaseContext dbContext;

        public MRController(
            IRenderer renderer, 
            IFileStorage fileStorage,
            DataBaseContext dbContext)
        {
            this.renderer = renderer;
            this.fileStorage = fileStorage;
            this.dbContext = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(List<IFormFile> files)
        {
            if (files?.Count < 1)
                return BadRequest();

            var record = new MrRecord
            {
                HospitalId = HospitalId,
            };
            dbContext.Add(record);
            await dbContext.SaveChangesAsync();
            
            await fileStorage.SaveMRData(record.Id, files.Select(f => f.OpenReadStream()));
            return Ok();
            //var tasks = files.Select(f => renderer.Render(f.OpenReadStream())).ToArray();
            //var folderName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            //var directoryInfo = Directory.CreateDirectory(folderName);
            //var memoryZipStream = new MemoryStream();
            //string zipFileName = "";
            //try
            //{
            //    await Task.WhenAll(tasks);
            //    foreach (var (picStream, FileName) in tasks.Zip(files, (task, f) => (task.Result, f.FileName)))
            //    {
            //        using (var fileStream = System.IO.File.OpenWrite(Path.Combine(folderName, $"{FileName}.png")))
            //        {
            //            await picStream.CopyToAsync(fileStream);
            //        }
            //    }
            //    zipFileName = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
            //    ZipFile.CreateFromDirectory(folderName, zipFileName);
            //    using (var fileZipStream = System.IO.File.OpenRead(zipFileName))
            //    {
            //        await fileZipStream.CopyToAsync(memoryZipStream);
            //        memoryZipStream.Position = 0;
            //    }
            //}
            //finally
            //{
            //    directoryInfo.Delete(recursive: true);
            //    if (!string.IsNullOrEmpty(zipFileName))
            //        System.IO.File.Delete(zipFileName);
            //}
            //return File(memoryZipStream, "application/zip", "dataset.zip");
        }

        [HttpGet("records")]
        public async Task<IActionResult> Records()
            => Json(await dbContext.MrRecords.ToListAsync());

        [HttpGet("records/{hospitalId:guid}")]
        public async Task<IActionResult> Records(Guid hospitalId)
            => Json(await dbContext.MrRecords.Where(r => r.HospitalId == hospitalId).ToListAsync());

        [HttpGet("algorithms")]
        public async Task<IActionResult> QueueAnalyzes()
            => Json(await dbContext.MrAlgorithms.ToListAsync());

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyzes()
            => Json(await dbContext.MrAnalyzes.ToListAsync());

        [HttpPost("analyze/queue/{mrRecordId:guid}")]
        public async Task<IActionResult> QueueAnalyzes(
            Guid mrRecordId,
            [FromBody]QueueAnalyzesRequest request)
        {
            var targetRecord = await dbContext.MrRecords.SingleOrDefaultAsync(r => r.Id == mrRecordId);
            if (targetRecord == null) return NotFound();

            var algs = await dbContext.MrAlgorithms.Where(a => request.Algorithms.Contains(a.Id)).ToListAsync();
            if (algs.Count != request.Algorithms.Count)
                return NotFound();

            var analyzes = algs.Select(a => new MrAnalyze
            {
                MrAlgorithm = a,
                MrRecord = targetRecord,
                Status = MrAnalyzeStatus.InQueue
            }).ToList();
            dbContext.AddRange(analyzes);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
