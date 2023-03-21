using System.ComponentModel.DataAnnotations;
namespace Project_1640.Models
{
    public class View
    {
        [Key]
        public int ViewId { get; set; }
        public string UserId { get; set; }
        public int IdeaId { get; set; }
        public DateTime ViewDate { get; set; }
    }
}
