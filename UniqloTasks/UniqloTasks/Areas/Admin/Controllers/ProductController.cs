using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloTasks.DataAccess;
using UniqloTasks.Extentions;
using UniqloTasks.ViewModels;

using UniqloTasks.ViewModels.Products;
using UniqloTasks.Models;

namespace UniqloTasks.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController(IWebHostEnvironment _env, UniqloDbContext _context) : Controller
    {
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Brand).Where(p => !p.IsDeleted).ToList();

            return View(products);
            //return RedirectToAction(nameof(Create));
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (vm.File != null)
            {
                if (!vm.File.IsValidType("image"))
                    ModelState.AddModelError("File", "File must be an image");
                if (!vm.File.IsValidSize(400))
                    ModelState.AddModelError("File", "File must be less than 400kb");
            }
            if (vm.OtherFiles.Any())
            {
                if (!vm.OtherFiles.All(x => x.IsValidType("image")))
                {
                    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidType("image")).Select(x => x.FileName));
                    ModelState.AddModelError("OtherFiles", fileNames + " is (are) not an image");
                }
                //if (!vm.OtherFiles.All(x => x.IsValidSize(400)))
                //{
                //    string fileNames = string.Join(',', vm.OtherFiles.Where(x => !x.IsValidSize(400)).Select(x => x.FileName));
                //    ModelState.AddModelError("OtherFiles", fileNames + " is (are) bigger than 400kb");
                //}
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                return View(vm);
            }
            if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
            {
                ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
                ModelState.AddModelError("BrandId", "Brand not found");
                return View();
            }
            Product product = vm;
            product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
            product.Images = vm.OtherFiles.Select(x => new ProductImage
            {
                ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
            }).ToList();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
