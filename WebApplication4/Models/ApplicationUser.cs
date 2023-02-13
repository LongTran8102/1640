
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateTime? DoB { get; set; }
        [Required]
        public string? Email { get; set; }        
        public string ? Phone { get; set; }
    }
}
