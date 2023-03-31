using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class MostPopularIdeaViewModel
    {
       
        public int IdeaId { get; set; }
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }
        public string UserId { get; set; }
        public string TopicName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalReaction { get; set; }
        public int? TotalLike { get; set; }
        public int? TotalDislike { get; set; }
        public List<MostPopularIdeaViewModel> List { get; set; }
        
    }
    public class MostPopularIdea
    {
        public List<MostPopularIdeaViewModel> List { get; set; }
    }
}
