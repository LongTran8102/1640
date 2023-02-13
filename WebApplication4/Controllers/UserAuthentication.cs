using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    public class UserAuthentication : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
