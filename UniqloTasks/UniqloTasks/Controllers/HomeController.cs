using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniqloTasks.DataAccess;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Home;
using UniqloTasks.ViewModels.Products;
using UniqloTasks.ViewModels.Sliders;
using UniqloTasks.ViewModels;


namespace UniqloTasks.Controllers
{
    public class HomeController(UniqloDbContext _context) : Controller
    {

        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();
            vm.Sliders = await _context.Sliders.Select(x => new SliderListItemVM
            {

                ImgUrl = x.ImageUrl,
                Link = x.Link,
                Subtitle = x.Subtitle,
                Title = x.Title,
            }).ToListAsync();
            vm.Products = await _context.Products.Select(x => new ProductListItemVM
            {
                CoverImage = x.CoverImage,
                Discount = x.Discount,
                Id = x.Id,
                IsInStock = x.Quantity > 0,
                Name = x.Name,
                SellPrice = x.SellPrice
            }).ToListAsync();

            return View(vm);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
