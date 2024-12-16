using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using UniqloTasks.DataAccess;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Basket;
using UniqloTasks.ViewModels.Brands;
using UniqloTasks.ViewModels.Products;
using UniqloTasks.ViewModels.Shop;

namespace UniqloTasks.Controllers
{
	public class ShopController : Controller
	{
		private readonly UniqloDbContext _context;
		private readonly UserManager<User> _userManager;


		public ShopController(UniqloDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
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

	
		public async Task<IActionResult> AddBasket(int id)
		{
			var basket = getBasket();
			var item = basket.FirstOrDefault(x => x.Id == id);
			if (item != null)
				item.Count++;
			else
			{
				basket.Add(new BasketCokiesItemVM
				{
					Id = id,
					Count = 1
				});
			}
			string data = JsonSerializer.Serialize(basket);
			HttpContext.Response.Cookies.Append("basket", data);
			return RedirectToAction("Index", "Home"); 

			return Ok();

		}

		public async Task<IActionResult> GetBasket()
		{
			return Json(getBasket());
		}
		List<BasketCokiesItemVM> getBasket()
		{
			try
			{
				string? value = HttpContext.Request.Cookies["basket"];
				if (value is null) return new();
				return JsonSerializer.Deserialize<List<BasketCokiesItemVM>>(value) ?? new();
			}
			catch (Exception)
			{
				return new();
			}
		}

		[HttpPost]
		public async Task<IActionResult> RemoveBasket(int id)
		{
			var basket = getBasket();
			var item = basket.FirstOrDefault(x => x.Id == id); 

			if (item != null)
			{
				item.Count--; 

				if (item.Count <= 0)
				{
					basket.Remove(item); 
				}
			}

			string data = JsonSerializer.Serialize(basket);
			HttpContext.Response.Cookies.Append("basket", data);

			return RedirectToAction("Index", "Home"); 
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
			var product = await _context.Products
		  .Include(p => p.Comments)
		  .FirstOrDefaultAsync(p => p.Id == id);

			if (product == null) return NotFound();
			var UserId = _userManager.GetUserId(User); 
			ViewBag.UserId = userId; 

			//return View(product);
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
		[HttpPost]
		public IActionResult AddComment(int productId, int userId, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return BadRequest("Comment can't be empty.");
			}

			var comment = new Comment
			{
				Text = text,
				UserId = userId,
				ProductId = productId
			};

			_context.Comments.Add(comment);
			_context.SaveChanges();

			return RedirectToAction("Details", new { id = productId });
		}
	}
}
