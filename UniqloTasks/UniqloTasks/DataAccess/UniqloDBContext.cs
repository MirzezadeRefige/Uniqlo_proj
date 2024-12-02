using Microsoft.EntityFrameworkCore;
using UniqloTasks.Models;

namespace UniqloTasks.DataAccess
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage>? Images { get; set; }

        public UniqloDbContext(DbContextOptions opt) : base(opt) { }
		
	}
}
