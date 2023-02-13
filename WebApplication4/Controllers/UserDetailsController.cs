using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    public class UserDetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
