using System.ComponentModel.DataAnnotations;

namespace TopicProject.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateTime ClosureDate { get; set; } = DateTime.Now;
        public DateTime FinalClosureDate { get; set; } = DateTime.Now;
    }
}
