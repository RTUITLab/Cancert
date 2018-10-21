using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Algorithm;

namespace WebApplication.Models.Data
{
    public class MrAnalyze
    {
        public Guid Id { get; set; }
        public MrAnalyzeStatus Status { get; set; }

        public Guid MrRecordId { get; set; }
        public MrRecord MrRecord { get; set; }

        public Guid MrAlgorithmId { get; set; }
        public MrAlgorithm MrAlgorithm { get; set; }

    }
}
