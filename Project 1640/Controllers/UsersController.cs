using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;

namespace Project_1640.Controllers
{
    public class UsersController : Controller
    {
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;

        public UsersController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }

        // GET: UsersController
        public ActionResult Index()
        {
            UsersViewModel usersViewModel = new UsersViewModel();
            usersViewModel.UserList = context.applicationUsers.ToList();
            return View(usersViewModel);
        }
    }
}
