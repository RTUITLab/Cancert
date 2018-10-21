using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DicomParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicAPI.Requests;
using PublicAPI.Responses;
using WebApplication.Controllers.Stuff;
using WebApplication.DataBase;
using WebApplication.Models.Data;
using WebApplication.Services.Analyzers;
using WebApplication.Services.FileSystem;
using WebApplication.Services.Render;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MRController : HospitalController
    {
        private readonly IMapper mapper;
        private readonly IRenderer renderer;
        private readonly IFileStorage fileStorage;
        private readonly DataBaseContext dbContext;

        public MRController(
            IMapper mapper,
            IRenderer renderer,
            IFileStorage fileStorage,
            DataBaseContext dbContext)
        {
            this.mapper = mapper;
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
        {
            var records = (await dbContext
                  .MrRecords
                  .ToListAsync())
                  .Select(r => (r, fileStorage.GetMRData(r.Id).Result))
                  .Select(r => new RecordView
                  {
                      Id = r.r.Id,
                      HospitalId = r.r.HospitalId,
                      PagesCount = r.Result.Count(),
                      Size = r.Result.Sum(s => s.Length),
                      Meta = Meta(r.Result.First()).ToList()
                  });

            return Json(records);
        }
        private static IEnumerable<Meta> Meta(Stream str)
        {
            var d = new DicomDecoder();
            d.Init(str);
            return d.GetMeta();
        }
        [HttpGet("records/{hospitalId:guid}")]
        public async Task<IActionResult> Records(Guid hospitalId)
            => Json(await dbContext
                .MrRecords
                .Where(r => r.HospitalId == hospitalId)
                .ProjectTo<RecordView>()
                .ToListAsync());

        [HttpGet("record/{recordId:guid}")]
        public async Task<IActionResult> Record(Guid recordId)
        {
            var record = await dbContext.MrRecords.Where(r => r.Id == recordId).SingleOrDefaultAsync();
            if (record == null)
                return NotFound();
            return Json(mapper.Map<RecordView>(record));
        }

        [AllowAnonymous]
        [HttpGet("record/{recordId:guid}/content")]
        public async Task<IActionResult> RecordContent(Guid recordId)
        {
            var dicom = await fileStorage.GetMRData(recordId);
            var targetFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(targetFolder);
            var zipFileName = "";
            try
            {
                var writes = dicom.Select(async (s, i) =>
                {
                    using (var fileStream = System.IO.File.OpenWrite(Path.Combine(targetFolder, $"{i}.dcm")))
                    {
                        await s.CopyToAsync(fileStream);
                    }
                }).ToArray();
                await Task.WhenAll(writes);
                zipFileName = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
                ZipFile.CreateFromDirectory(targetFolder, zipFileName);
                using (var fileStream = System.IO.File.OpenRead(zipFileName))
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    return File(memoryStream.ToArray(), "application/zip", "dataset.zip");
                }
            }
            finally
            {
                Directory.Delete(targetFolder, recursive: true);
                System.IO.File.Delete(zipFileName);
            }
        }


        [HttpGet("algorithms")]
        public async Task<IActionResult> QueueAnalyzes()
            => Json(await dbContext.MrAlgorithms.ToListAsync());

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyzes()
            => Json(await dbContext.MrAnalyzes.ToListAsync());

        [AllowAnonymous]
        [HttpPost("analyze/accept/{analyzeId:guid}")]
        public async Task<IActionResult> AcceptAnalyze(
            Guid analyzeId,
            List<IFormFile> pics)
        {
            if (pics?.Count < 1)
                return BadRequest();

            var analyze = await dbContext.MrAnalyzes.SingleOrDefaultAsync(a => a.Id == analyzeId);
            if (analyze == null)
            {
                Console.WriteLine("NO ANALYZE");
                return NotFound();
            }

            await fileStorage.SaveAnalyzeResult(analyze.Id, pics.Select(p => p.OpenReadStream()));
            analyze.Status = MrAnalyzeStatus.Ready;
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [AllowAnonymous]

        [HttpGet("analyze/result/{analyzeId:guid}/{pageNum:int}")]
        public async Task<IActionResult> GetResultPage(
            Guid analyzeId, int pageNum)
        {
            var stream = await fileStorage.GetAnalyzeResult(analyzeId, pageNum);
            if (stream == null)
                return NotFound();
            return File(stream, "omage/png", $"{pageNum}.png");
        }


        [HttpPost("analyze/queue/{mrRecordId:guid}")]
        public async Task<IActionResult> QueueAnalyzes(
            Guid mrRecordId,
            [FromBody]QueueAnalyzesRequest request,
            [FromServices] IAnalyzer analyzer)
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
            await Task.WhenAll(analyzes.Select(a => analyzer.Analyze(a)));
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("analyze/startWork/{analyzeId:guid}")]
        public async Task<IActionResult> UpdateStatus(Guid analyzeId)
        {
            var analyze = await dbContext.MrAnalyzes.SingleOrDefaultAsync(a => a.Id == analyzeId);
            if (analyze == null)
            {
                Console.WriteLine("NO ANALYZE");
                return NotFound();
            }
            analyze.Status = (MrAnalyzeStatus)50;
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
