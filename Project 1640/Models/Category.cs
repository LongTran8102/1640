using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1640.Models
{
<<<<<<< HEAD
    [Table("Categories")]
=======
    [Table("Category")]
>>>>>>> LongTran
    public class Category
    {
        [Key]
        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
