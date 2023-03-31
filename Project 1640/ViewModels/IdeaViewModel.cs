using Project_1640.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project_1640.ViewModels
{
    public class IdeaViewModel
    {
        public int IdeaId { get; set; }
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }   
        public string UserId { get; set; }    
        public int? TotalView { get; set; }
        public string TopicName { get; set; }
        public DateTime CreatedDate { get; set; }
        public IFormFile AttachFile { get; set; }
        [Display(Name = "I accept the above terms and conditions.")]
        public bool TermsConditions { get; set; } = false;
        public IQueryable<Idea> Ideas { get; set; }
        public IQueryable<IdeaViewModel> Idea { get; set; }

        public string CreatedDateSortOrder { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Term { get; set; }
        public string OrderBy { get; set; }
        
    }
}
