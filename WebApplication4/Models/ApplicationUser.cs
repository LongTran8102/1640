using Microsoft.AspNetCore.Identity;

namespace WebApplication4.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string ? ProfilePicture { get; set; }

    }
}
