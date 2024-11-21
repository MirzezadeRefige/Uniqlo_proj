using Microsoft.AspNetCore.Mvc;

namespace UniqloTasks.Controllers
{

    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
