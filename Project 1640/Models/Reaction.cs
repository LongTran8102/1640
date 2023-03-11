using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_1640.Models
{
    [Table("Reaction")]
    public class Reaction
    {
        [Key]
        public int ReactionId { get; set; }
        public bool React { get; set; }
        public string UserId { get; set; }
        public int IdeaId { get; set; }
    }
}
