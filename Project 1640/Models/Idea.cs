using System.ComponentModel.DataAnnotations;

namespace Project_1640.Models
{
    public class Idea
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string IdeaName { get; set; }
    }
}
