using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloTasks.DataAccess;
using UniqloTasks.ViewModels.Brands;
using UniqloTasks.ViewModels.Products;
using UniqloTasks.ViewModels.Shops;

namespace UniqloTasks.Controllers
{
	public class ShopController(UniqloDbContext _context) : Controller
	{

		public async Task<IActionResult> Index(int? catID,string amount)
		{
			var query = _context.Products.AsQueryable();
			if (catID.HasValue) 
			{
				query = query.Where(x => x.BrandId == catID);
			}
			if (amount != null) 
			{
				var prices = amount.Split('-')
					.Select(x => Convert.ToInt32(x));
				query = query.Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
			}
			ShopVM vM = new ShopVM();
			vM.Brands = await _context.Brands
				.Where(x => !x.IsDeleted)
				.Select(x => new BrandAndProductVM
				{
					Id = x.Id,
					Name = x.Name,
					Count = x.Products.Count
				})
				.ToListAsync();
			vM.Products = await query
				.Take(5)
				.Select(x => new ProductListItemVM
				{
					CoverImage = x.CoverImage,
					Discount = x.Discount,
					Id = x.Id,
					IsInStock = x.Quantity > 0,
					Name = x.Name,
					SellPrice = x.SellPrice
				}).ToListAsync();
			vM.ProductCount =await query.CountAsync();
			return View(vM);
		}
	}
}
