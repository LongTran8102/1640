using System.ComponentModel.DataAnnotations;

namespace Project_1640.Models
{
    public class Idea
    {
        [Key]
        public int IdeaId { get; set; }
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }
        public string UserId { get; set; }
        public string? FilePath { get; set; }
    }
}
