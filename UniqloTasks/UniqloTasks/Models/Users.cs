using Microsoft.AspNetCore.Identity;

namespace UniqloTasks.Models
{
	public class Users : IdentityUser
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string ProfileImageUrl { get; set; }

	}
}
