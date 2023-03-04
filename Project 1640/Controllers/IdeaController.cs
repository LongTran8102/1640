using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Migrations;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Net.Mail;
using System.Security.Claims;
using Idea = Project_1640.Models.Idea;
using MailKit.Net.Smtp;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.BC;
using System.Text;

namespace Project_1640.Controllers
{
    public class IdeaController : Controller
    {
        public readonly IWebHostEnvironment webHostEnvironment;        
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;

        public static string Topic_Id;
        public static string Topic_Name;

        public IdeaController(IWebHostEnvironment _webHostEnvironment, ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            userManager = _userManager;
            webHostEnvironment = _webHostEnvironment;
            context = _context;
        }
        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 5;
            return View(PaginatedList<Idea>.Create(context.Ideas.ToList(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            GetTopicName(id);
            var idea = GetIdeaByID(id);
            return View(idea);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            DropDownList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdeaViewModel model, Email emailData, int id)
        {
            //Create Idea
            GetTopicId(id);
            DropDownList();            
            Idea idea = new Idea()
            {
                TopicId = Topic_Id,
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

            SendMail(emailData, id);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var idea = GetIdeaByID(id);            
            EditIdeaViewModel model = new()
            {                
                IdeaName = idea.IdeaName,
                IdeaDescription = idea.IdeaDescription,
                CategoryId = idea.CategoryId,
                UserId = userManager.GetUserId(HttpContext.User),
                ExsitingFile = idea.FilePath
            };
            DropDownList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditIdeaViewModel model)
        {           
            DropDownList();
                Idea idea = GetIdeaByID(model.ID);
                idea.IdeaName = model.IdeaName;
                idea.IdeaDescription = model.IdeaDescription;               
                idea.CategoryId = model.CategoryId;
                if (model.AttachFile != null)
                {
                    if (idea.FilePath != null)
                    {
                        string ExitingFile = Path.Combine(webHostEnvironment.WebRootPath, "UserFiles", idea.FilePath);
                        System.IO.File.Delete(ExitingFile);
                    }
                    idea.FilePath = UploadFile(model.AttachFile);
                }
                var SelectedIdea = context.Ideas.Attach(idea);
                SelectedIdea.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                
                return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null || context.Ideas == null)
            {
                return NotFound();
            }

            var idea = GetIdeaByID(id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            Idea idea = GetIdeaByID(id);
            if (idea.FilePath != null)
            {
                string ExitingFile = Path.Combine(webHostEnvironment.WebRootPath, "UserFiles", idea.FilePath);
                System.IO.File.Delete(ExitingFile);
            }
            if (idea != null)
            {
                context.Ideas.Remove(idea);
                context.SaveChanges();
            }     
            
            return RedirectToAction("index");
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
            string UniqueFileName = formFile.FileName;
            string TargetPath = Path.Combine(webHostEnvironment.WebRootPath, "UserFiles", UniqueFileName);
            using (var stream = new FileStream(TargetPath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            return UniqueFileName;
        }

        public void SendMail(Email emailData, int id)
        {
            //Send Mail
            var userId = userManager.GetUserId(HttpContext.User);
            foreach (var user in context.Users)
            {
                if (user.Id == userId)
                {
                    emailData.To = user.Email;
                }
            }

            emailData.From = "luandtgcs200115@fpt.edu.vn";
            emailData.Password = "Conso123!";
            emailData.Body = "Thanks for submitting";

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailData.From));
            email.To.Add(MailboxAddress.Parse(emailData.To));
            email.Subject = "Create Idea Success Notification";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };

            using var smtp = new SmtpClient();
            //smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(emailData.From, emailData.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public Idea GetIdeaByID(int id)
        {
            return context.Ideas.FirstOrDefault(x => x.IdeaId == id);
        }

        public void GetTopicId (int id)
        {
            foreach(var topic in context.Topics)
            {
                if(topic.Id == id)
                {
                    Topic_Id = Convert.ToString(topic.Id);
                }
            }
        }

        public void GetTopicName(int id)
        {
            foreach(var topic in context.Topics)
            {
                if (topic.Id == id)
                {
                    Topic_Name = topic.Name;
                }
            }
            ViewBag.TopicName = Topic_Name;
        }

        [HttpGet]
        public FileResult DownloadCSV(int id)
        {
            //Take all idea in topic
            List<Idea> ideaList = new List<Idea>();
            foreach (var idea in context.Ideas)
            {
                if (id == Convert.ToInt32(idea.TopicId))
                {
                    ideaList.Add(idea);
                }
            }

            //Add to csv and print csv
            string csv = string.Empty;
            string[] ColumnName = new string[] { "IdeaId", "IdeaName", "CreatedDate", "IdeaDescription", "CategoryId ", "TopicId", "FilePath" };
            foreach(string columnName in ColumnName)
            {
                csv += columnName + ','; 
            }
            csv += "\r\n";
            foreach(var idea in ideaList)
            {
                csv += idea.IdeaId.ToString().Replace(',', ',') + ',';
                csv += idea.IdeaName.Replace(',', ',') + ',';
                csv += idea.CreatedDate.ToString().Replace(',', ',') + ',';
                csv += idea.IdeaDescription.Replace(',', ',') + ',';
                csv += idea.CategoryId.Replace(',', ',') + ',';
                csv += idea.TopicId.Replace(',', ',') + ',';
                csv += idea.FilePath.Replace(',', ',') + ',';

                csv += "\r\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "Idea.csv");
        }
    }
}
