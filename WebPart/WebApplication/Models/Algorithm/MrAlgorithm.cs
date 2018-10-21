using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Data;

namespace WebApplication.Models.Algorithm
{
    public class MrAlgorithm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public List<MrAnalyze> MrAnalyzes { get; set; }
    }
}
