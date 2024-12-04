using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Auths;

namespace UniqloTasks.Controllers
{
	public class AccountController(UserManager<User> _userManager) : Controller
	{
		public IActionResult Register()
		{
			return View();
		}
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM vm)
		{
			if (!ModelState.IsValid)

				return View();
			User user = new User
			{
				Fullname = vm.Username,
				Email = vm.Email,
				UserName = vm.Username,

			};
		var result = await _userManager.CreateAsync(user, vm.Password);
			if (!result.Succeeded)
			{
				foreach (var err in result.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
				return View();
			}
			if (!ModelState.IsValid) return View();
			return RedirectToAction("Index", "Home");


		}
	}
	}
