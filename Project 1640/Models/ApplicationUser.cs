using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1640.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [ForeignKey("DepartmentId ")]
        public int DepartmentId { get; set; }
    }
}
