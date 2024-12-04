using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniqloTasks.Models;
namespace UniqloTasks.DataAccess
{

<<<<<<< HEAD
	public class UniqloDbContext : IdentityDbContext
    {
		public DbSet<Slider> Sliders { get; set; }
		public DbSet<Brand> Brands { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductImage>? Images { get; set; }
		public DbSet<User> Users { get; set; }



		public UniqloDbContext(DbContextOptions opt) : base(opt) { }
=======
        public UniqloDbContext(DbContextOptions opt) : base(opt) { }
		
>>>>>>> f2ff803b16bf6ed640bd6f993403a9f432cfb703
	}
}
