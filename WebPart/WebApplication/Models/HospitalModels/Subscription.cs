using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.HospitalModels
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }

        public DateTime End { get; set; }

        public Guid HospitalId { get; set; }
        public Hospital Hospital { get; set; }
    }
}
