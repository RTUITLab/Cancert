using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Algorithm;
using WebApplication.Models.Data;
using WebApplication.Models.HospitalModels;

namespace WebApplication.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<MrRecord> MrRecords { get; set; }
        public DbSet<MrAnalyze> MrAnalyzes{ get; set; }
        public DbSet<MrAlgorithm> MrAlgorithms { get; set; }

        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}
