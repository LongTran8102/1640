using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class CommentViewModel
    {
        public Idea Ideas { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
