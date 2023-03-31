using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Diagnostics;

namespace Project_1640.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            _logger = logger;
            context = _context;
            userManager = _userManager;
        }

        public IActionResult Index(string term = "", string orderBy = "", int currentPage = 1)
        {
            if (userManager.GetUserId(HttpContext.User) != null)
            {
                term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
                var ideaData = new IdeaViewModel();
                ViewData["DateSort"] = string.IsNullOrEmpty(orderBy) ? "date" : "";
                var ideas = (from idea in context.Ideas
                             where (userManager.GetUserId(HttpContext.User) == idea.UserId && term == "") || (idea.IdeaName.ToLower().StartsWith(term) && userManager.GetUserId(HttpContext.User) == idea.UserId)
                             select new Idea
                             {
                                 IdeaId = idea.IdeaId,
                                 IdeaName = idea.IdeaName,
                                 IdeaDescription = idea.IdeaDescription,
                                 FilePath = idea.FilePath,
                                 CreatedDate = idea.CreatedDate,
                                 TotalLike = idea.TotalLike,
                                 TotalDislike = idea.TotalDislike,
                                 TotalView = idea.TotalView,
                             });
                switch (orderBy)
                {
                    case "date":
                        ideas = ideas.OrderBy(a => a.CreatedDate);
                        break;
                    default:
                        ideas = ideas.OrderByDescending(a => a.CreatedDate);
                        break;
                }
                var totalRecords = ideas.Count();
                var pageSize = 5;
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
                ideas = ideas.Skip((currentPage - 1) * pageSize).Take(pageSize);
                ideaData.Ideas = ideas;
                ideaData.CurrentPage = currentPage;
                ideaData.TotalPages = totalPages;
                ideaData.PageSize = pageSize;
                ideaData.Term = term;
                ideaData.OrderBy = orderBy;
                return View(ideaData);
            }
            return RedirectToAction("Account", "Identity", new { id = "Login" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}