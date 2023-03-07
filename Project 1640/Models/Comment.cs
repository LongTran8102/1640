using System.ComponentModel.DataAnnotations;

namespace Project_1640.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public string UserId { get; set; }
        public int IdeaId { get; set; }
    }
}
