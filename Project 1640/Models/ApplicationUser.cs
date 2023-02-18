using Microsoft.AspNetCore.Identity;
<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> 966e45f38d108640538bfb026a22d5c4c6c4295b

namespace Project_1640.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
<<<<<<< HEAD
=======

        [ForeignKey("DepartmentId ")]
        public int DepartmentId { get; set; }
>>>>>>> 966e45f38d108640538bfb026a22d5c4c6c4295b
    }
}
