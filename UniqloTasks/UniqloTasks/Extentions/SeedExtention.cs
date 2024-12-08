﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using UniqloTasks.Models;
using UniqloTasks.Views.Account.Enums;

namespace UniqloTasks.Extentions
{
	public static class SeedExtention
	{
		public static void UseUserSeed(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
				CreateRoles(roleManager).Wait();
				CreateUsers(userManager).Wait();
			}
		}
		private static async Task CreateRoles(RoleManager<IdentityRole> _roleManager)
		{
			if (!await _roleManager.Roles.AnyAsync())
			{
				foreach (Roles item in Enum.GetValues(typeof(Roles)))
				{
					await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));
				}
			}
		}
		private static async Task CreateUsers(UserManager<User> _userManager)
		{
			if (!await _userManager.Users.AnyAsync(u => u.NormalizedUserName == "ADMIN"))
			{
				User user = new User();
				user.UserName = "admin";
				user.Email = "admin@gmail.com";
				user.Fullname = "admin";
				string role = nameof(Roles.Admin);
				await _userManager.CreateAsync(user, "123");
				await _userManager.AddToRoleAsync(user, role);
			}
		}
	}
}
