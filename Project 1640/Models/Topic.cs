using System.ComponentModel.DataAnnotations;

namespace Project_1640.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime ClosureDate { get; set; } = DateTime.Now;
        public DateTime FinalClosureDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
