using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    [Table("Category")]
    public class Category
    {
        [Key,Required]
        public int Category_Id { get; set; }

        public string Category_Name { get; set; }
    }
}
