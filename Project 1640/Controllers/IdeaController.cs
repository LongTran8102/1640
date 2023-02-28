using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Security.Claims;

namespace Project_1640.Controllers
{
    public class IdeaController : Controller
    {
        public readonly IWebHostEnvironment webHostEnvironment;        
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;

        public IdeaController(IWebHostEnvironment _webHostEnvironment, ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            userManager = _userManager;
            webHostEnvironment = _webHostEnvironment;
            context = _context;
        }
        public IActionResult Index()
        {

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            DropDownList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdeaViewModel model)
        {
            DropDownList();
            Idea idea = new Idea()
            {

                IdeaName = model.IdeaName,
                IdeaDescription = model.IdeaDescription,
                CategoryId = model.CategoryId, 
                UserId = userManager.GetUserId(HttpContext.User),     
            };

            if (model.AttachFile != null)
            {
                idea.FilePath = UploadFile(model.AttachFile);
            }

            context.Ideas.Add(idea);
            context.SaveChanges();
            return RedirectToAction("Create");

        }
        
        public void DropDownList()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            foreach (var cat in context.Category)
            {
                category.Add(new SelectListItem { Text = cat.CategoryName, Value = Convert.ToString(cat.CategoryId) });
            }
            ViewBag.CategoryList = category;
        }




        private string UploadFile(IFormFile formFile)
        {
            string UniqueFileName = Guid.NewGuid().ToString() + "-" + formFile.FileName;
            string TargetPath = Path.Combine(webHostEnvironment.WebRootPath, "UserFiles", UniqueFileName);
            using (var stream = new FileStream(TargetPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            return UniqueFileName;
        }
    }
}
