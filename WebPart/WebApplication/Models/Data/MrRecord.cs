using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.HospitalModels;

namespace WebApplication.Models.Data
{
    public class MrRecord
    {
        public Guid Id { get; set; }


        public Guid HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public List<MrAnalyze> MyProperty { get; set; }
    }
}
