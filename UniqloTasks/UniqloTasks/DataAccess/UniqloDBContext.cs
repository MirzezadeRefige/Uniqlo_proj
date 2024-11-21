using System.Collections.Generic;
using UniqloTasks.Models;

using Microsoft.EntityFrameworkCore;

namespace UniqloTasks.DataAccess
{


    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }

        public UniqloDbContext(DbContextOptions<UniqloDbContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}