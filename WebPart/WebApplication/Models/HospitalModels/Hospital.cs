using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Data;

namespace WebApplication.Models.HospitalModels
{
    public class Hospital
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Subscription> Subscriptions { get; set; }
        public List<MrRecord> MrRecords { get; set; }
    }
}
