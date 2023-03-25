using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Collections.ObjectModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project_1640.Controllers
{
    public class UsersController : Controller
    {
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;
        public readonly RoleManager<IdentityRole> roleManager;
        public string RoleName;
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
            DropDownList();
            if (id == null || context.applicationUsers == null)
            {
                return NotFound();
            }
            var User = await context.applicationUsers.FindAsync(id);
            var viewUser = (from user in context.applicationUsers
                            join ur in context.UserRoles on user.Id equals ur.UserId
                            join r in context.Roles on ur.RoleId equals r.Id
                            where user.Id == id
                            select new UsersViewModel
                            {
                                UserID = id,
                                RoleId = r.Id,
                                FirstName = User.Firstname,
                                LastName = User.Lastname,
                                Email = User.Email,
                                DepartmentId = User.DepartmentId,
                            });

            UsersViewModel view = new UsersViewModel();
            foreach(var views in viewUser)
            {
                view.UserID = views.UserID;
                view.RoleId = views.RoleId;
                view.FirstName = views.FirstName;
                view.LastName = views.LastName;
                view.Email = views.Email;
                view.DepartmentId = views.DepartmentId;
            }
            return View(view);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(UsersViewModel model, string id)
        {
            DropDownList();
            var user = await context.applicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userWithSameEmail = await userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != id)
            {
                ModelState.AddModelError("Email", "This email is already used");
                return View(model);
            }

            if (model.RoleId == "--Please select role--")
            {
                return View(model);
            }

            user.UserName = model.Email;
            user.Firstname = model.FirstName;
            user.Lastname = model.LastName;
            user.Email = model.Email;
            user.DepartmentId = model.DepartmentId;
            await userManager.UpdateAsync(user);


            var users = await userManager.FindByIdAsync(id);
            var role = await roleManager.FindByIdAsync(model.RoleId);
            var oldrole = await userManager.GetRolesAsync(users);
            var newrole = await roleManager.GetRoleNameAsync(role);
            await userManager.RemoveFromRolesAsync(user,oldrole);
            await userManager.AddToRoleAsync(user,newrole);
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
        
        private bool UserExists(string id)
        {
            return context.applicationUsers.Any(e => e.Id == id);
        }
    }
}
