using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Migrations;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Security.Claims;
using MailKit.Net.Smtp;
using System.Net.Mail;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Idea = Project_1640.Models.Idea;
using Comment = Project_1640.Models.Comment;
using Topic = Project_1640.Models.Topic;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.BC;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.ExtendedProperties;
using System;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Org.BouncyCastle.Crypto;
using DocumentFormat.OpenXml.Office2010.Excel;
using ICSharpCode.SharpZipLib.Zip;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Project_1640.Controllers
{
    [Authorize]
    public class IdeaController : Controller
    {
        public readonly IWebHostEnvironment webHostEnvironment;
        private IHostingEnvironment oIHostingEnvironment;
        public readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;

        public static string Topic_Id;
        public static DateTime Topic_ClosureDate;
        public static DateTime Topic_FinalClosureDate;

        public IdeaController(IHostingEnvironment _IHostingEnvironment, IWebHostEnvironment _webHostEnvironment, ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            userManager = _userManager;
            webHostEnvironment = _webHostEnvironment;
            oIHostingEnvironment = _IHostingEnvironment;
            context = _context;
        }

        //Pagination Page and Search
        public IActionResult Index(string term = "", string orderBy = "", int currentPage = 1)
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            var ideaData = new IdeaViewModel();
            ideaData.CreatedDateSortOrder = string.IsNullOrEmpty(orderBy) ? "date_desc" : "";
            var ideas = (from idea in context.Ideas
                         where term == "" || idea.IdeaName.ToLower().StartsWith(term)
                         select new Idea
                         {
                             IdeaId = idea.IdeaId,
                             IdeaName = idea.IdeaName,
                             IdeaDescription = idea.IdeaDescription,
                             FilePath = idea.FilePath,
                             CreatedDate = idea.CreatedDate,
                             TotalLike = idea.TotalLike,
                             TotalDislike = idea.TotalDislike,
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
            ideaData.Ideas = ideas;
            ideaData.CurrentPage = currentPage;
            ideaData.TotalPages = totalPages;
            ideaData.PageSize = pageSize;
            ideaData.Term = term;
            ideaData.OrderBy = orderBy;
            return View(ideaData);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id, Idea idea)
        {
            CountView(id);
            idea = GetIdeaByID(id);
            List<Comment> comments = new List<Comment>();
            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach (var user in context.applicationUsers)
            {
                users.Add(user);
            }

            foreach (var comment in context.Comments)
            {
                if (comment.IdeaId == idea.IdeaId)
                {
                    foreach (var user in users)
                    {
                        if (user.Id == comment.UserId)
                        {
                            comment.IdeaId = idea.IdeaId;
                            comment.UserId = user.Firstname;
                        }
                    }
                    comments.Add(comment);
                }
            }

            int like = 0;
            int dislike = 0;

            foreach (var react in context.Reactions)
            {
                if (idea.IdeaId == react.IdeaId)
                {
                    if (react.React == true)
                    {
                        like++;
                    }
                    if (react.React == false)
                    {
                        dislike++;
                    }
                }
            }
            int TotalView = 0;

            foreach (var view in context.Views)
            {
                if (view.IdeaId == id)
                    TotalView++;
            }
            idea.TotalLike = like;
            idea.TotalDislike = dislike;
            idea.TotalView = TotalView;
            context.Ideas.Update(idea);
            await context.SaveChangesAsync();

            CommentViewModel viewModel = new CommentViewModel();
            viewModel.Ideas = idea;
            viewModel.Comments = comments;
            return View(viewModel);
        }

        //GET Create Idea
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            foreach (var topic in context.Topics)
            {
                if (topic.Id == id)
                {
                    Topic_ClosureDate = topic.ClosureDate;
                }
            }
            if (Topic_ClosureDate > DateTime.Now)
            {
                DropDownList();
                return View();
            }
            return RedirectToAction("Index", "Topic");
        }

        //POST Create Idea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdeaViewModel model, Email emailData, Idea idea, int id)
        {
            //Create Idea
            DropDownList();
            if (model.TermsConditions == true)
            {
                idea.TopicId = id.ToString();
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
                await context.SaveChangesAsync();
                //Send Mail
                SendMailCreateIdea(emailData, idea);
                return RedirectToRoute(new { controller = "Topic", action = "Details", id });
            }
            else
            {
                ViewBag.FailAccept = "Please accept Term & Condition";
            }
            return View();
        }

        //GET Edit Idea
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var idea = GetIdeaByID(id);
            if (userManager.GetUserId(HttpContext.User) == idea.UserId || this.User.IsInRole("Admin") == true)
            {
                DropDownList();

                EditIdeaViewModel model = new()
                {
                    IdeaId = idea.IdeaId,
                    IdeaName = idea.IdeaName,
                    IdeaDescription = idea.IdeaDescription,
                    CategoryId = idea.CategoryId,
                    UserId = userManager.GetUserId(HttpContext.User),
                    ExsitingFile = idea.FilePath,
                };
                return View(model);
            }
            return RedirectToAction("Index");
        }

        //POST Edit Idea
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

        //GET Delete Idea
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null || context.Ideas == null)
            {
                return NotFound();
            }
            var idea = GetIdeaByID(id);
            if (userManager.GetUserId(HttpContext.User) == idea.UserId || this.User.IsInRole("Admin") == true)
            {
                if (idea == null)
                {
                    return NotFound();
                }
                return View(idea);
            }
            return RedirectToAction("Index");
        }

        //POST Delete Idea
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

        //Take Category Dropdown List
        public void DropDownList()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            foreach (var cat in context.Category)
            {
                category.Add(new SelectListItem { Text = cat.CategoryName, Value = Convert.ToString(cat.CategoryId) });
            }
            ViewBag.CategoryList = category;
        }

        //Take Attach File
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

        //Donwload Attach File
        public IActionResult DownloadFile(int id)
        {
            Idea idea = GetIdeaByID(id);
            if (idea.FilePath != null)
            {
                var path = Path.Combine(webHostEnvironment.WebRootPath, "UserFiles", idea.FilePath);
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                var contentType = "APPLICATION/octet-stream";
                var fileName = Path.GetFileName(path);
                return File(memory, contentType, fileName);
            }
            return RedirectToAction("Index");
        }

        //Send Mail After Creating Idea
        public void SendMailCreateIdea(Email emailData, Idea idea)
        {
            //Take submiter details
            var userId = userManager.GetUserId(HttpContext.User);
            ApplicationUser user = new ApplicationUser();
            foreach (var users in context.applicationUsers)
            {
                if (users.Id == userId)
                {
                    emailData.To = users.Email;
                    user = users;
                }
            }
            string topicName = "";
            foreach (var topic in context.Topics)
            {
                if (topic.Id == Convert.ToInt32(idea.TopicId))
                {
                    topicName = topic.Name;
                }
            }
            string categoryName = "";
            foreach (var category in context.Category)
            {
                if (category.CategoryId == Convert.ToInt32(idea.CategoryId))
                {
                    categoryName = category.CategoryName;
                }
            }
            //Format email form
            string BodyMessage =
                "You had create a new idea in topic " + $"{topicName}" + " successfully\r\n\r\n" +
                "<table style=\"border: 1px solid;\">\r\n" +
                "   <tr style=\"border: 1px solid;\">\r\n " +
                "       <td style=\"border: 1px solid;\">\r\n" +
                "           Submitter\r\n" +
                "       </td>\r\n" +
                "       <td style=\"border: 1px solid;\">\r\n" +
                $"         {user.Firstname}\r\n" +
                "       </td>\r\n" +
                "   </tr>\r\n" +
                "   <tr style=\"border: 1px solid;\">\r\n " +
                "       <td style=\"border: 1px solid;\">\r\n" +
                "           Category\r\n" +
                "       </td>\r\n" +
                "       <td style=\"border: 1px solid;\">\r\n" +
                $"         {categoryName}\r\n" +
                "       </td>\r\n" +
                "   </tr>\r\n" +
                "   <tr style=\"border: 1px solid;\">\r\n " +
                "       <td style=\"border: 1px solid;\">\r\n" +
                "           Created Date\r\n" +
                "       </td>\r\n" +
                "       <td style=\"border: 1px solid;\">\r\n" +
                $"         {idea.CreatedDate}\r\n" +
                "       </td>\r\n" +
                "   </tr>\r\n" +
                "   <tr style=\"border: 1px solid;\">\r\n " +
                "       <td style=\"border: 1px solid;\">\r\n" +
                "           Idea Description\r\n" +
                "       </td>\r\n" +
                "       <td style=\"border: 1px solid;\">\r\n" +
                $"         {idea.IdeaDescription}\r\n" +
                "       </td>\r\n" +
                "   </tr>\r\n" +
                "   <tr style=\"border: 1px solid;\">\r\n " +
                "       <td style=\"border: 1px solid;\">\r\n" +
                "           File Path\r\n" +
                "       </td>\r\n" +
                "       <td style=\"border: 1px solid;\">\r\n" +
                $"         {idea.FilePath}\r\n" +
                "       </td>\r\n" +
                "   </tr>\r\n" +
                "</table>";
            //Input email details
            emailData.From = "luandtgcs200115@fpt.edu.vn";
            emailData.Password = "Conso123!";
            emailData.Body = BodyMessage;
            var email = new MimeMessage();
            {
                email.From.Add(MailboxAddress.Parse(emailData.From));
                email.To.Add(MailboxAddress.Parse(emailData.To));
                email.Subject = "Create Idea Success Notification";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailData.Body };
            }
            using var smtp = new SmtpClient();
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(emailData.From, emailData.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        //Download idea file
        public FileResult ZipFile(int id)
        {
            var webRoot = oIHostingEnvironment.WebRootPath;
            var fileName = "MyZip.zip";
            var tempOutput = webRoot + "UserFiles" + fileName;
            using (ZipOutputStream oZipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                oZipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var FileList = new List<string>();
                foreach (var idea in context.Ideas)
                {
                    if (idea.IdeaId == id)
                    {
                        if (idea.FilePath != null)
                        {
                            FileList.Add(webRoot + "/UserFiles/" + idea.FilePath);
                        }
                    }
                }

                for (int i = 0; i < FileList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(FileList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    oZipOutputStream.PutNextEntry(entry);
                    using (FileStream oFileStream = System.IO.File.OpenRead(FileList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            oZipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                oZipOutputStream.Finish();
                oZipOutputStream.Flush();
                oZipOutputStream.Close();
            }
            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }
            if (finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing found"));
            }
            return File(finalResult, "application/zip", fileName);
        }

        //Take Idea By id
        public Idea GetIdeaByID(int id)
        {
            return context.Ideas.FirstOrDefault(x => x.IdeaId == id);
        }

        //Take Topic Id by id
        public void GetTopicIdFromIdea(int id)
        {
            foreach (var idea in context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    Topic_Id = Convert.ToString(idea.TopicId);
                }
            }
        }
        public void CountView(int id)
        {
            GetTopicIdFromIdea(id);
            foreach (var topics in context.Topics)
            {
                if (topics.Id.ToString() == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
                int count = 0;
                foreach (var views in context.Views)
                {
                    if (views.IdeaId == id && views.UserId == userManager.GetUserId(HttpContext.User))
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    View view = new View();
                    view.IdeaId = id;
                    view.ViewDate = DateTime.Now;
                    view.UserId = userManager.GetUserId(HttpContext.User);
                    context.Views.Add(view);
                }
            }
        }
    }
}
