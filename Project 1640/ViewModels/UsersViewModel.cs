using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class UsersViewModel
    {
        public List<ApplicationUser> UserList { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string DepartmentName { get; set; }
    }
}
