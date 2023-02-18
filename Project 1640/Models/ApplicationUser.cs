using Microsoft.AspNetCore.Identity;

namespace Project_1640.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
