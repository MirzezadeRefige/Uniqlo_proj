﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using UniqloTasks.Models;
using UniqloTasks.Service.Abstracts;
using UniqloTasks.ViewModels.Auths;
using UniqloTasks.Views.Account.Enums;

namespace UniqloTasks.Controllers
{
	public class AccountController(UserManager<User> _userManager, SignInManager<User> _signInManager, RoleManager<IdentityRole> _roleManger, IEmailService _service) : Controller
	{
		
		private bool isAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
		public IActionResult Register()
		{
			if (isAuthenticated) return RedirectToAction("Index", "Home");

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM vm)
		{
			if (isAuthenticated) return RedirectToAction("Index", "Home");

			if (!ModelState.IsValid) return View();
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
			var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
			if (!roleResult.Succeeded)
			{
				foreach (var err in roleResult.Errors)
				{
					ModelState.AddModelError("", err.Description);
				}
				return View();
			}
			string token =  await _userManager.GenerateEmailConfirmationTokenAsync(user);
			_service.SendEmailConfirmation(user.Email, user.UserName, token);
			return Content("Email sent!");


			//return RedirectToAction("Login", "Account");
			return View();

		}
		//public async Task<IActionResult> roleMethod() 
		//{
		//	foreach (Roles item in Enum.GetValues(typeof(Roles)))
		//	{
		//		await _roleManger.CreateAsync(new IdentityRole(item.GetRole()));
		//	}
		//	return Ok();
		//}


		public async Task<IActionResult> Login()
		{
			if (isAuthenticated) return RedirectToAction("Index", "Home");

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
		{
			if (isAuthenticated) return RedirectToAction("Index", "Home");

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
					//return View(vm);


				}
				//return RedirectToAction("Index", "Home");

				return View(vm);
			}

			if (string.IsNullOrWhiteSpace(returnUrl))
			{
				if (await _userManager.IsInRoleAsync(user, "Admin"))
				{
					return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
				}
				return RedirectToAction("Index", "Home");
			}
			return LocalRedirect(returnUrl);
		}
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}
		public async Task<IActionResult> VerifyEmail(string token, string user)
		{
			var entity = await _userManager.FindByNameAsync(user);
			if (entity is null) return BadRequest();
			var result = await _userManager.ConfirmEmailAsync(entity, token);
			if (!result.Succeeded) 
			{
			StringBuilder sb = new StringBuilder();
				foreach (var item in result.Errors) 
				{
					sb.AppendLine(item.Description);	
				}
				return Content(sb.ToString());
			}
			await _signInManager.SignInAsync(entity, true);
			return RedirectToAction("Index", "Home");

		
		}
	}
}
