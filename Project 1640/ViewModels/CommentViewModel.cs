using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public Idea Ideas { get; set; }
        public List<Comment>? ListComments { get; set; }
    }
}
