using Microsoft.AspNetCore.Identity;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;
=======
>>>>>>> LongTran

namespace Project_1640.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
<<<<<<< HEAD

        [ForeignKey("DepartmentId ")]
        public int DepartmentId { get; set; }
=======
>>>>>>> LongTran
    }
}
