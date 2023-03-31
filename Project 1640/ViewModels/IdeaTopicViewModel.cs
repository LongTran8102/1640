using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class IdeaTopicViewModel
    { 
        public Topic Topics { get; set; }
        public IQueryable<Idea> Ideas { get; set; }
    }
}
