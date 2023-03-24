using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public readonly RoleManager<IdentityRole> roleManager;

        public UsersController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
        }

        // GET: UsersController
        public async Task<IActionResult> Index()
        {

            var users = (from user in context.applicationUsers
                         join d in context.Department on user.DepartmentId equals d.DepartmentId
                         join ur in context.UserRoles on user.Id equals ur.UserId
                         join r in context.Roles on ur.RoleId equals r.Id
                         select new UsersViewModel

                         {
                             UserID = user.Id,
                             FirstName = user.Firstname,
                             LastName = user.Lastname,
                             Email = user.Email,
                             Roles = r.Name,
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
            DropDownList();
            var user = await context.applicationUsers.FindAsync(id);
            var viewUser = new UsersViewModel

            {
                UserID = id,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                DepartmentId = user.DepartmentId,
            };
            return View(viewUser);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> Edit(UsersViewModel model, string id)
        {
            DropDownList();
            var user = await context.applicationUsers.FindAsync(id);
            if (user == null)
                return NotFound();
            var userWithSameEmail = await userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != id)
            {
                ModelState.AddModelError("Email", "This email is already used");
                return View(model);
            }
            user.UserName = model.Email;
            user.Firstname = model.FirstName;
            user.Lastname = model.LastName;
            user.Email = model.Email;
            user.DepartmentId = model.DepartmentId;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await context.applicationUsers.FindAsync(id);
            if (id == null || context.applicationUsers == null)
            {
                return NotFound();
            }
            DropDownList();
            foreach (var userid in context.UserRoles)
            {
                if (userid.UserId == id)
                {
                    var role = userid.RoleId;
                    var viewUser = new UsersViewModel

                    {
                        UserID = id,
                        RoleId = role,
                    };
                    return View(viewUser);
                };
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<object> EditRole(UsersViewModel model, string id)
        {
            string userid = "";
            string roleid = "";
            DropDownList();           
            foreach (var user in context.UserRoles)
            {
                if (user.UserId == id)
                {
                   
                    roleid=user.RoleId
                    
                };
            }
            
            if (userid == id)
            {
                model.RoleId = roleid;
            }
            model.RoleId = userid;
            userid.UserId = id;
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || context.applicationUsers == null)
            {
                return NotFound();
            }

            var viewUser = (from user in context.applicationUsers
                            join d in context.Department on user.DepartmentId equals d.DepartmentId
                            join ur in context.UserRoles on user.Id equals ur.UserId
                            join r in context.Roles on ur.RoleId equals r.Id
                            where user.Id == id
                            select new UsersViewModel


                            {
                                UserID = id,
                                FirstName = user.Firstname,
                                LastName = user.Lastname,
                                Email = user.Email,
                                Roles = r.Name,
                                DepartmentName = d.DepartmentName,
                            });
            return View(viewUser);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await context.applicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.applicationUsers.Remove(user);


            context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool UserExists(string id)
        {
            return context.applicationUsers.Any(e => e.Id == id);
        }
        public void DropDownList()
        {
            List<SelectListItem> department = new List<SelectListItem>();
            foreach (var dep in context.Department)
            {
                department.Add(new SelectListItem { Text = dep.DepartmentName, Value = Convert.ToString(dep.DepartmentId) });
            }
            ViewBag.DepartList = department;
            List<SelectListItem> roles = new List<SelectListItem>();
            foreach (var rol in context.Roles)
            {
                roles.Add(new SelectListItem { Text = rol.Name, Value = Convert.ToString(rol.Id) });
            }
            ViewBag.RolesList = roles;
        }
    }
}
