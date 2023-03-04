using Project_1640.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Project_1640.ViewModels
{
    public class IdeaViewModel
    {
        public string IdeaName { get; set; }
        public string IdeaDescription { get; set; }
        public string CategoryId { get; set; }   
        public string UserId { get; set; }        
        public IFormFile AttachFile { get; set; }
        [Display(Name = "I accept the above terms and conditions.")]
        [CheckBoxRequired(ErrorMessage = "Please accept the terms and condition.")]
        public bool TermsConditions { get; set; }
        public IQueryable<Idea> Ideas { get; set; }
        public string IdeaNameSortOrder { get; set; }
        public string CreatedDateSortOrder { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Term { get; set; }
        public string OrderBy { get; set; }

    }
}
