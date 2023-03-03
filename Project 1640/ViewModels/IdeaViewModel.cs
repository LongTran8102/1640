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
    }
}
