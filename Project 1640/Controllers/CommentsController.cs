using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Project_1640.Data;
using Project_1640.Models;

namespace Project_1640.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public static DateTime Topic_FinalClosureDate;
        public static int Topic_Id;

        public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.ToListAsync());
        }

        //GET Details Comment
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        //GET Create Comment
        public IActionResult Create(int id)
        {
            GetTopicIdFromIdea(id);
            foreach (var topics in _context.Topics)
            {
                if (topics.Id == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
                foreach (var idea in _context.Ideas)
                {
                    if (idea.IdeaId == id)
                    {
                        ViewBag.CommentId = idea.IdeaId;
                    }
                }
                return View();
            }
            return RedirectToRoute(new { controller = "Idea", action = "Details", id });
        }

        //POST Create Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Email emailData, Comment comment, int id)
        {
            GetTopicIdFromIdea(id);
            foreach (var topics in _context.Topics)
            {
                if (topics.Id == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
                comment.UserId = _userManager.GetUserId(HttpContext.User);
                comment.IdeaId = id;
                comment.CommentDate = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();

                SendMailReceiveComment(emailData, id);
                return RedirectToRoute(new { controller = "Idea", action = "Details", id });
            }
            return RedirectToRoute(new { controller = "Idea", action = "Details", id });
        }

        //GET Edit Comment
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        //POST Edit Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        //GET Delete Comment
        public async Task<IActionResult> Delete(int id)
        {
            GetTopicIdFromComment(id);
            foreach (var topics in _context.Topics)
            {
                if (topics.Id == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }
            var comment = await _context.Comments.FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
            }
            return RedirectToAction("Index");
        }

        //POST Delete Comment
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            GetTopicIdFromComment(id);
            foreach (var topics in _context.Topics)
            {
                if (topics.Id == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
              
                if (_context.Comments == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Comments'  is null.");
                }
                var comment = _context.Comments.Find(id);

                if (comment != null)
                {
                    _context.Comments.Remove(comment);

                }
                _context.SaveChanges();
                return RedirectToAction("Details", "Idea", new { id = comment.IdeaId });
            }
            return RedirectToAction("Index", "Idea");
        }

        public void GetTopicIdFromComment(int id)
        {
            int IdeaId = 0;
            foreach(var comment in _context.Comments)
            {
                if(comment.CommentId == id)
                {
                    IdeaId = comment.IdeaId;
                }
            }

            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == IdeaId)
                {
                    Topic_Id = Convert.ToInt32(idea.TopicId);
                }
            }
        }

        public void GetTopicIdFromIdea(int id)
        {
            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    Topic_Id = Convert.ToInt32(idea.TopicId);
                }
            }
        }
        //Send Mail After Creating Idea
        private void SendMailReceiveComment(Email emailData, int id)
        {
            //Take topic's author details
            string userId = "";
            string topicName = "";
            Idea Idea = new Idea();
            ApplicationUser User = new ApplicationUser();

            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    userId = idea.UserId;
                    Idea = idea;
                }
            }

            foreach (var topic in _context.Topics)
            {
                if (topic.Id.ToString() == Idea.TopicId)
                {
                    topicName = topic.Name;
                }
            }

            foreach (var user in _context.applicationUsers)
            {
                if (user.Id == userId)
                {
                    User = user;
                }
            }

            //Format email form
            string BodyMessage = "You had received a new comment in the idea " + $"{Idea.IdeaName}" + " in the topic " + $"{topicName}" + ". Did you read it?";

            //Input email details
            emailData.To = User.Email;
            emailData.From = "ideacollectionsender@gmail.com";
            emailData.Password = "obvzsinreznekrtn";
            emailData.Body = BodyMessage;
            var email = new MimeMessage();
            {
                email.From.Add(MailboxAddress.Parse(emailData.From));
                email.To.Add(MailboxAddress.Parse(emailData.To));
                email.Subject = "New Comment Notification";
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

        //Check comment exist
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
