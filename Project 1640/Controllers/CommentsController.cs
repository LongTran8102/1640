using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;

namespace Project_1640.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public static DateTime Topic_FinalClosureDate;
        public static string Topic_Id;

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
            foreach (var topics in _context.Topics)
            {
                if (topics.Id.ToString() == Topic_Id)
                {
                    Topic_FinalClosureDate = topics.FinalClosureDate;
                }
            }
            if (Topic_FinalClosureDate > DateTime.Now)
            {
                return View();
            }
            return RedirectToRoute(new { controller = "Idea", action = "Details", id });

        }

        //POST Create Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment, int id)
        {
            GetTopicId(id);

            comment.UserId = _userManager.GetUserId(HttpContext.User);
            comment.IdeaId = id;
            comment.CommentDate = DateTime.Now;
            _context.Add(comment);
            await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
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

        //POST Delete Comment
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Check comment exist
        private bool CommentExists(int id)
        {
          return _context.Comments.Any(e => e.CommentId == id);
        }
        public void GetTopicId(int id)
        {
            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    Topic_Id = Convert.ToString(idea.TopicId);
                }
            }
        }
    }
}
