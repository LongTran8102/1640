using Project_1640.Migrations;
using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class CategoryViewModel
    {
        public IQueryable<Category> categories { get; set; }
         public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }    
        public string Term { get; set; }
    }
}
