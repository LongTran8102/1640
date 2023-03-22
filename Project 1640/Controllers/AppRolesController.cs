using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Project_1640.Models;
using System.Data;
using System.Collections.Generic;

namespace Project_1640.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AppRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        //List all the Roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            //avoid duplicate role
            //if(!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            //Check id and database exist
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }

            //Take component in database
            var role = await _roleManager.Roles.FirstOrDefaultAsync(m => m.Id == id);

            //Check if database has value
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //Check if database exists
            if (_roleManager.Roles == null)
            {
                return Problem("Entity set 'RoleManager<IdentityRole>.Roles'  is null.");
            }

            //Create variable stores details of component need delete
            IdentityRole roles = new IdentityRole();

            //Find value need to be deleted compares to id 
            foreach (var role in _roleManager.Roles)
            {
                if (id == role.Id)
                {
                    roles = role;
                }
            }

            //Delete roles
            await _roleManager.DeleteAsync(roles);
            return RedirectToAction("Index");
        }
    }
}
