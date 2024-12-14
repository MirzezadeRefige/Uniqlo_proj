using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqloTasks.DataAccess;
using UniqloTasks.ViewModels.Basket;
using UniqloTasks.ViewModels.Brands;
using UniqloTasks.ViewModels.Products;
using UniqloTasks.ViewModels.Shop;
using UniqloTasks.ViewModels.Shop;

namespace UniqloTasks.Controllers
{
	public class ShopController : Controller
	{
		private readonly UniqloDbContext _context;

		public ShopController(UniqloDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(int? catId, string amount)
		{
			var query = _context.Products.AsQueryable();
			if (catId.HasValue)
			{
				query = query.Where(x => x.BrandId == catId);
			}
			if (amount != null)
			{
				var prices = amount.Split('-').Select(x => Convert.ToInt32(x));
				query = query
					.Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
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
				.Take(6)
				.Select(x => new ProductListItemVM
				{
					CoverImage = x.CoverImage,
					Discount = x.Discount,
					Id = x.Id,
					IsInStock = x.Quantity > 0,
					Name = x.Name,
					SellPrice = x.SellPrice
				})
				.ToListAsync();
			vM.ProductCount = await query.CountAsync();
			return View(vM);
		}

		// Add item to basket (cookie-based)
		// Add item to basket (cookie-based)
		public async Task<IActionResult> AddBasket(int id)
		{
			var basket = GetBasketFromCookie(); // Get the basket from the cookies
			var item = basket.FirstOrDefault(x => x.Id == id);

			if (item != null)
				item.Count++;  // Increment item count
			else
			{
				basket.Add(new BasketCokiesItemVM { Id = id, Count = 1 });  // Add new item to basket
			}

			// Serialize the basket and save it back to the cookies
			string data = JsonSerializer.Serialize(basket);
			HttpContext.Response.Cookies.Append("basket", data);
			return Ok();
		}

		// Get basket items
		public async Task<IActionResult> GetBasket()
		{
			return Json(GetBasketFromCookie());  // Get basket data from cookies and return it
		}

		// Helper function to get basket from cookies
		private List<BasketCokiesItemVM> GetBasketFromCookie()
		{
			try
			{
				string? value = HttpContext.Request.Cookies["basket"];
				if (value == null) return new List<BasketCokiesItemVM>();  // Return empty list if no basket exists
				return JsonSerializer.Deserialize<List<BasketCokiesItemVM>>(value) ?? new List<BasketCokiesItemVM>();  // Deserialize the basket
			}
			catch (Exception)
			{
				return new List<BasketCokiesItemVM>();  // Return empty list in case of an error
			}
		}


		public async Task<IActionResult> Details(int? id)
		{
			if (!id.HasValue) return BadRequest();
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("Login", "Account");

			var data = await _context.Products
				.Include(x => x.Images)
				.Include(x => x.ProductRatings)
				.Where(x => x.Id == id.Value && !x.IsDeleted)
				.FirstOrDefaultAsync();

			if (data == null) return NotFound();

			string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
			if (userId != null)
			{
				var rating = await _context.ProductRatings
					.Where(x => x.UserId == userId && x.ProductId == id)
					.Select(x => x.RatingRate)
					.FirstOrDefaultAsync();

				ViewBag.Rating = rating == 0 ? 5 : rating;
			}
			else
			{
				ViewBag.Rating = 5;
			}

			return View(data);
		}
		[Authorize]
		public async Task<IActionResult> Rate(int? productId, int rate = 1)
		{
			if (!productId.HasValue) return BadRequest();
			string UserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
			if (!await _context.Products.AnyAsync(p => p.Id == productId)) return NotFound();
			var rating = await _context.ProductRatings.Where(x => x.ProductId == productId && x.UserId == UserId).FirstOrDefaultAsync();
			if (rating is null)
			{
				await _context.ProductRatings.AddAsync(new Models.ProductRating
				{
					ProductId = productId.Value,
					RatingRate = rate,
					UserId = UserId
				});
			}
			else
			{
				rating.RatingRate = rate;
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Details), new { id = productId });
		}
	}
}
