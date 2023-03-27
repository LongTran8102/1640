// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_1640.Data;
using Project_1640.Models;

namespace Project_1640.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ApplicationDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            context = _context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>


        private async Task LoadAsync(IdentityUser user)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            string roleId = "";
            int depId = 0;

            //Take user info
            foreach (var users in context.applicationUsers)
            {
                if (userId == users.Id)
                {
                    FirstName = users.Firstname;
                    LastName = users.Lastname;
                    Email = users.Email;
                    depId = users.DepartmentId;
                }
            }
            //Take department name
            foreach (var dep in context.Department)
            {
                if (dep.DepartmentId == depId)
                {
                    Department = dep.DepartmentName;
                }
            }
            //Take role id
            foreach (var userRole in context.UserRoles)
            {
                if (userRole.UserId == userId)
                {
                    roleId = userRole.RoleId;
                }
            }
            //Take role name
            foreach (var role in context.Roles)
            {
                if (roleId == role.Id)
                {
                    Role = role.Name;
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
    }
}
