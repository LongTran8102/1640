using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {

            var users = (from user in context.applicationUsers
                         join d in context.Department on user.DepartmentId equals d.DepartmentId
                         select new UsersViewModel

                         {
                             UserID = user.Id,
                             FirstName = user.Firstname,
                             LastName = user.Lastname,
                             Email = user.Email,
                             Roles = userManager.GetRolesAsync(user).Result,
                             DepartmentName = d.DepartmentName,


                         }).ToArray();
            return View(users);
        }

        // GET: UsersController
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || context.applicationUsers == null)
            {
                return NotFound();
            }

            var user = await context.applicationUsers.FindAsync(id);
            var viewUser = new UsersViewModel

            {
                UserID = id,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
            };
            return View(viewUser);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsersViewModel model, string id)
        {           

            var user = await context.applicationUsers.FindAsync(id);
            if(user==null)
                return NotFound();
            var userWithSameEmail = await context.applicationUsers.FindAsync(model.Email);
            /*if(userWithSameEmail!=null&& userWithSameEmail.Id != model.UserID)
            {
                ModelState.AddModelError("Email", "This email is already used");
                    return View(model);
            }*/
            user.Firstname= model.FirstName;
            user.Lastname= model.LastName;  
            user.Email= model.Email;
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return context.applicationUsers.Any(e => e.Id == id);
        }

    }
}
