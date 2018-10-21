using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models.Hospital
{
    public class Hospital
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
