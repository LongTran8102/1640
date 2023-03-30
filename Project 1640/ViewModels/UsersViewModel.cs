using Microsoft.AspNetCore.Identity;
using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class UsersViewModel
    {
        public List<ApplicationUser> UserList { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string RoleId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IdentityUser User { get; set; }
        public IQueryable<UsersViewModel> Users { get; set; }
        public string Term { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
