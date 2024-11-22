using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using UniqloTasks.Models;

public class UniqloDbContextFactory : DbContext
{

    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public UniqloDbContextFactory(DbContextOptions opt) : base(opt) { }
}
//public class UniqloDbContextFactory : IDesignTimeDbContextFactory<UniqloDbContext>
//{
//    public UniqloDbContext CreateDbContext(string[] args)
//    {
//        var configuration = new ConfigurationBuilder()
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json")
//            .Build();

//        var optionsBuilder = new DbContextOptionsBuilder<UniqloDbContext>();
//        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

//        return new UniqloDbContext(optionsBuilder.Options);
//    }
//}