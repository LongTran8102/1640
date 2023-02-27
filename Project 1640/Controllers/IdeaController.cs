using Microsoft.AspNetCore.Mvc;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;

namespace Project_1640.Controllers
{
    public class IdeaController : Controller
    {
        public readonly IWebHostEnvironment webHostEnvironment;
        //public readonly IFileRepository fileRepository;
        public readonly ApplicationDbContext context;
        public IdeaController(IWebHostEnvironment _webHostEnvironment, ApplicationDbContext _context)
        {

            webHostEnvironment = _webHostEnvironment;
            context = _context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IdeaViewModel model)
        {
            if (ModelState.IsValid)
            {
                Idea idea = new()
                {
                    IdeaName = model.IdeaName,
                    IdeaDescription = model.IdeaDescription,
                };
                if (model.AttachFile != null)
                {
                    idea.FilePath = UploadFile(model.AttachFile);
                }
                //fileRepository.Index(file);
                context.Ideas.Add(idea);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
