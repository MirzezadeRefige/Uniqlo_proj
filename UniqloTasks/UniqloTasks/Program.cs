using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqloTasks.DataAccess;
using UniqloTasks.Models;

namespace UniqloTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<UniqloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSql"));
            });
            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            { 
            opt.User.RequireUniqueEmail = true; 
            opt.Password.RequiredLength = 5;
				opt.Password.RequireDigit = false;
				opt.Password.RequireLowercase = false;
				opt.Password.RequireUppercase = false;
				opt.Lockout.MaxFailedAccessAttempts = 2;
                opt.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(5);
			}).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();

            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();


            app.MapControllerRoute(
                name: "area",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
