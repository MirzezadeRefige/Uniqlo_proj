﻿using Microsoft.AspNetCore.Identity;

namespace UniqloTasks.Models
{
	public class User : IdentityUser
	{

		public string Fullname { get; set; }
		public string? Address { get; set; }
		public string? ProfileImageUrl { get; set; }
		public ICollection<ProductRating>? ProductRatings { get; set; }



	}
}
