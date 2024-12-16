using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniqloTasks.DataAccess;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Brands;

namespace UniqloTasks.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BrandController : Controller
	{
		private readonly UniqloDbContext _context;

		public BrandController(UniqloDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			List<Brand> Brands = await _context.Brands.ToListAsync();
			return View(Brands);
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Brand brand)
		{
			if (!ModelState.IsValid)
			{
				return View(brand);
			}

			var newBrand = new Brand
			{
				Name = brand.Name
			};

			_context.Brands.Add(newBrand);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> Update(int id)
		{
			var brand = await _context.Brands.FindAsync(id);
			if (brand is null) { return NotFound(); }
			return View(brand);
		}
		[HttpPost]
		public async Task<IActionResult> Update(Brand brand)
		{
			if (!ModelState.IsValid)
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					Console.WriteLine(error.ErrorMessage);
				}
				return View(brand);
			}
			var updatedBrand = await _context.Brands.FindAsync(brand.Id);

			if (updatedBrand == null)
			{
				return NotFound();
			}
			updatedBrand.Name = brand.Name;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));


		}
		public async Task<IActionResult> Delete(int id)
		{
			var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);
			if (brand is not null)
			{
				_context.Brands.Remove(brand);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return NotFound();
		}

		public async Task<IActionResult> Hide(int id)
		{
			var slide = _context.Brands.FirstOrDefault(s => s.Id == id);
			if (slide != null)
			{
				slide.IsDeleted = true;
				_context.SaveChanges();
			}
			return RedirectToAction(nameof(Index));

		}
		public async Task<IActionResult> Show(int id)
		{
			var slide = _context.Brands.FirstOrDefault(s => s.Id == id);
			if (slide == null)
			{
				
				return NotFound();
			}

			slide.IsDeleted = false;
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}


	}
}
