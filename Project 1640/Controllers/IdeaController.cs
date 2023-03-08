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
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;

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
        public IActionResult Index(string term = "", string orderBy = "", int currentPage = 1)
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();

            var ideaData = new IdeaViewModel();
            ideaData.CreatedDateSortOrder = string.IsNullOrEmpty(orderBy) ? "date_desc" : "";

            var ideas = (from idea in context.Ideas
                         where term == "" || idea.IdeaName.ToLower().StartsWith(term)
                         select new Idea
                         {
                             IdeaName = idea.IdeaName,
                             IdeaDescription = idea.IdeaDescription,
                             FilePath = idea.FilePath,
                             CreatedDate = idea.CreatedDate,
                             IdeaId = idea.IdeaId
                         });

            switch (orderBy)
            {
                case "date_desc":
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
            //int pageSize = 5;
            //return View(PaginatedList<Idea>.Create(context.Ideas.ToList(), pageNumber ?? 1, pageSize));
            ideaData.Ideas = ideas;
            ideaData.CurrentPage = currentPage;
            ideaData.TotalPages = totalPages;
            ideaData.PageSize = pageSize;
            ideaData.Term = term;
            ideaData.OrderBy = orderBy;

            return View(ideaData);
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
        public async Task<IActionResult> Create(IdeaViewModel model, Email emailData, Idea idea, int id)
        {
            //Create Idea
            GetTopicId(id);
            DropDownList();

            if (model.TermsConditions == true)
            {

                idea.TopicId = Topic_Id;
                idea.IdeaName = model.IdeaName;
                idea.IdeaDescription = model.IdeaDescription;
                idea.CategoryId = model.CategoryId;
                idea.UserId = userManager.GetUserId(HttpContext.User);
                idea.CreatedDate = DateTime.Now;

                if (model.AttachFile != null)
                {
                    idea.FilePath = UploadFile(model.AttachFile);
                }

                context.Ideas.Add(idea);
                context.SaveChanges();

                //Send Mail
                SendMail(emailData, idea);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.FailAccept = "Please accept Term & Condition";

            }
            return View();

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var idea = GetIdeaByID(id);
            DropDownList();

            EditIdeaViewModel model = new()
            {
                IdeaName = idea.IdeaName,
                IdeaDescription = idea.IdeaDescription,
                CategoryId = idea.CategoryId,
                UserId = userManager.GetUserId(HttpContext.User),
                ExsitingFile = idea.FilePath
            };
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

        [HttpPost, ActionName("Delete")]
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
            return RedirectToAction("Index");
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

        public void SendMail(Email emailData, Idea idea)
        {
            var userId = userManager.GetUserId(HttpContext.User);

            foreach (var user in context.Users)
            {
                if (user.Id == userId)
                {
                    emailData.To = user.Email;
                }
            }

            string topicName = null;
            foreach(var topic in context.Topics)
            {
                if(topic.Id == Convert.ToInt32(idea.TopicId))
                {
                    topicName = topic.Name;
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("In the topic " + $"{topicName}"
                + "</br>"+ "You had upload the idea " + $"{idea.IdeaName}" + " successfully.");


            emailData.From = "luandtgcs200115@fpt.edu.vn";
            emailData.Password = "Conso123!";
            emailData.Body = stringBuilder.ToString();

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailData.From));
            email.To.Add(MailboxAddress.Parse(emailData.To));
            email.Subject = "Create Idea Success Notification";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(emailData.From, emailData.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public Idea GetIdeaByID(int id)
        {
            return context.Ideas.FirstOrDefault(x => x.IdeaId == id);
        }

        public void GetTopicId(int id)
        {
            foreach (var topic in context.Topics)
            {
                if (topic.Id == id)
                {
                    Topic_Id = Convert.ToString(topic.Id);
                }
            }
        }

        public void GetTopicName(int id)
        {
            foreach (var topic in context.Topics)
            {
                if (topic.Id == id)
                {
                    Topic_Name = topic.Name;
                }
            }
            ViewBag.TopicName = Topic_Name;
        }

        public FileResult CSVFile(int id)
        {
            //Find idea
            List<Idea> ideaList = new List<Idea>();

            foreach (var idea in context.Ideas)
            {
                if (Convert.ToInt32(idea.TopicId) == id)
                {
                    ideaList.Add(idea);
                }
            }

            string CSV = string.Empty;
            string[] columnName = new string[] { "IdeaId", "IdeaName", "IdeaDescription", "CreatedDate", "CategoryId", "TopicId", "FilePath" };

            foreach (var column in columnName)
            {
                CSV += column + ',';
            }

            CSV += "\r\n";

            foreach (var idea in ideaList)
            {
                CSV += idea.IdeaId.ToString().Replace(",", ",") + ',';
                CSV += idea.IdeaName.Replace(",", ",") + ',';
                CSV += idea.IdeaDescription.Replace(",", ",") + ',';
                CSV += idea.CreatedDate.ToString().Replace(",", ",") + ',';
                CSV += idea.CategoryId.Replace(",", ",") + ',';
                CSV += idea.TopicId.Replace(",", ",") + ',';
                CSV += idea.FilePath?.Replace(",", ",") + ',';

                CSV += "\r\n";
            }

            byte[] bytes = Encoding.UTF8.GetBytes(CSV);
            return File(bytes, "text/csv", "IdeasList.csv");
        }

        public IActionResult ExcelFile(int id)
        {
            //Find idea
            List<Idea> ideaList = new List<Idea>();

            foreach (var idea in context.Ideas)
            {
                if (Convert.ToInt32(idea.TopicId) == id)
                {
                    ideaList.Add(idea);
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Topic");
                var currentRow = 1;

                //Add title
                worksheet.Cell(currentRow, 1).Value = "IdeaId";
                worksheet.Cell(currentRow, 2).Value = "IdeaName";
                worksheet.Cell(currentRow, 3).Value = "IdeaDescription";
                worksheet.Cell(currentRow, 4).Value = "CreatedDate";
                worksheet.Cell(currentRow, 5).Value = "CategoryId";
                worksheet.Cell(currentRow, 6).Value = "FilePath";

                //Add details
                foreach (var idea in ideaList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = idea.IdeaId;
                    worksheet.Cell(currentRow, 2).Value = idea.IdeaName;
                    worksheet.Cell(currentRow, 3).Value = idea.IdeaDescription;
                    worksheet.Cell(currentRow, 4).Value = idea.CreatedDate;
                    worksheet.Cell(currentRow, 5).Value = idea.CategoryId;
                    worksheet.Cell(currentRow, 6).Value = idea.FilePath;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Topic.xlsx");
                }
            }
        }
    }
}
