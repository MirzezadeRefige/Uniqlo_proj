using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;
using UniqloTasks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UniqloTasks.Models;

public class UniqloDbContext : DbContext
            {
                public DbSet<Slider> Sliders { get; set; }
    public UniqloDbContext(DbContextOptions opt) : base(opt) { }
    
            
    }

    //public UniqloDbContext(DbContextOptions<UniqloDbContext> options)
    //    : base(options)
    //{
    //}

//public class UniqloDbContext : DbContext
//{
//    private readonly IConfiguration _configuration;

//    public DbSet<Slider> Sliders { get; set; }

//    public UniqloDbContext(DbContextOptions<UniqloDbContext> options, IConfiguration configuration)
//        : base(options)
//    {
//        _configuration = configuration;
//    }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        if (!optionsBuilder.IsConfigured)
//        {
//            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
//        }
//        base.OnConfiguring(optionsBuilder);
//    }
//}


//public class UniqloDbContext : DbContext
//{
//    private readonly IConfiguration _configuration;

//    public DbSet<Slider> Sliders { get; set; }

//    public UniqloDbContext(DbContextOptions<UniqloDbContext> options, IConfiguration configuration)
//        : base(options)
//    {
//        _configuration = configuration;
//    }

//    public UniqloDbContext(DbContextOptions options) : base(options)
//    {
//    }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        if (!optionsBuilder.IsConfigured)
//        {
//            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
//        }
//        base.OnConfiguring(optionsBuilder);
//    }
//}
