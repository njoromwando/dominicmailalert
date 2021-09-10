using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertSystem.Api
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AlertsItem> AlertsItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlertsItem>()
                .HasNoKey();
        }
    }

    public class AlertsItem
    {
        public string ItemCode { get; set; }
        public string ItemDescrip { get; set; }
    }
}