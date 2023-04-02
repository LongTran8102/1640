using Project_1640.Models;
namespace Project_1640.ViewModels
{
    public class TopicViewModel
    {
        public IQueryable<Topic> Topics { get; set; }
        public string CreatedDateSortOrder { get; set; }
        public string NameSort { get; set; }
        public string ClosureDateSort { get; set; }
        public string FinalClosureDateSort { get; set; }
        public string Term { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string OrderBy { get; set; }
    }
}
