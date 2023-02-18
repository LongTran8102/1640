using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1640.Models
{
<<<<<<< HEAD
    [Table("Category")]
=======
    [Table("Categories")]
>>>>>>> 966e45f38d108640538bfb026a22d5c4c6c4295b
    public class Category
    {
        [Key]
        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
