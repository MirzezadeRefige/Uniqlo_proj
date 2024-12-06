using Microsoft.AspNetCore.Mvc;

namespace UniqloTasks.Controllers
{
	public class ContactController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
