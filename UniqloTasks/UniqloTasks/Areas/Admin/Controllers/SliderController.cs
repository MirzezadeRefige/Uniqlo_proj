using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniqloTasks.DataAccess;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Sliders;
 
namespace UniqloTasks.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SliderCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (!vm.File.ContentType.StartsWith("image"))
            {
                ModelState.AddModelError("File", "Format type must be image");
                return View(vm);
            }
            if (vm.File.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "File size must be less than 2 mb");
                return View(vm);
            }
            string newFileName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);

            using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newFileName)))
            {
                await vm.File.CopyToAsync(stream);
            }
            Slider slider = new Slider
            {
                ImageUrl = newFileName,
                Title = vm.Title,
                Subtitle = vm.Subtitle!,
                Link = vm.Link
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

//        var options = new DbContextOptionsBuilder<UniqloDbContext>()
//.UseSqlServer("Server=DESKTOP-9OJ3NSG\\SQLEXPRESS;Database=UniqloProject;Trusted_Connection=True;TrustServerCertificate=True;")
//.Options;
//        using (UniqloDbContext _context1 = new(options))
//        {
//            _context1.Sliders.ToList();
//            _context1.Sliders.Add(new Models.Slider { });
//            _context1.SaveChanges();