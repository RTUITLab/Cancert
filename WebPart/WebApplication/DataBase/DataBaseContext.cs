using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models.Hospital;

namespace WebApplication.DataBase
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DataBaseContext(DbContextOptions options) : base(options)
        {
        }   
    }
}
