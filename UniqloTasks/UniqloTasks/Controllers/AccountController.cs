using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqloTasks.Models;
using UniqloTasks.ViewModels.Auths;

namespace UniqloTasks.Controllers
{
	public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager) : Controller
	{
		public IActionResult Register()
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


		public async Task<IActionResult> Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM vm)
		{
			if (!ModelState.IsValid) return View(vm);


			User? user = null;
			if (vm.UsernameOrEmail.Contains("@"))
			{
				user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
			}
			else
			{
				user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
			}
			if (user is null)
			{
				ModelState.AddModelError("", "username or password is wrong.");
				return View(vm);
				//return RedirectToAction("Index", "Home");

			}

			var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
			if (!result.Succeeded)
			{
				if (result.IsLockedOut)
				{
					ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
					//return RedirectToAction("Index", "Home");
					return View(vm);


				}
				if (result.IsNotAllowed)
				{
					ModelState.AddModelError("", "You must confirm your account");
					//return RedirectToAction("Index", "Home");
					return View(vm);


				}
				//return RedirectToAction("Index", "Home");

				return View(vm);
			}
			return RedirectToAction("Index","Home");

		}
		
	}
}
