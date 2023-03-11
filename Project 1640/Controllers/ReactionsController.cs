using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;
using System.Security.Claims;

namespace Project_1640.Controllers
{
    public class ReactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Like(int id, int Like = 0)
        {
            int count = 0;
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Reaction> reactionList = new List<Reaction>();
            foreach (var reaction in _context.Reactions)
            {
                reactionList.Add(reaction);
            }

            // Check if the user has already upvoted the idea
            var existingVote = await _context.Reactions.SingleOrDefaultAsync(v => v.UserId.ToString() == userId && v.IdeaId == id);

            if (existingVote != null)
            {
                foreach (var reaction in _context.Reactions)
                {
                    if(reaction.UserId == userId)
                    {
                        reaction.React = true;
                    }
                }
            }
            else
            {
                // User has not upvoted this idea, so record their upvote
                var react = new Reaction
                {
                    UserId = Convert.ToString(userId),
                    IdeaId = id,
                    React = true,
                };
                _context.Reactions.Add(react);

            }

            await _context.SaveChangesAsync();
            GetAllLike(id, Like);

            // Redirect back to the idea details page
            return RedirectToAction("Details");
        }

        public async Task<IActionResult> Dislike(int id, int DisLike = 0)
        {
            int count = 0;
            if()
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user has already upvoted the idea
            var existingVote = await _context.Reactions.SingleOrDefaultAsync(v => v.UserId.ToString() == userId && v.IdeaId == id);

            if (existingVote != null)
            {
                foreach (var reaction in _context.Reactions)
                {
                    if (reaction.UserId == userId)
                    {
                        reaction.React = false;
                    }
                }
            }
            else
            {
                // User has not downvoted this idea, so record their upvote
                var react = new Reaction
                {
                    UserId = Convert.ToString(userId),
                    IdeaId = id,
                    React = false,
                };

                _context.Reactions.Add(react);
            }

            await _context.SaveChangesAsync();
            GetAllDisLike(id, DisLike);

            // Redirect back to the idea details page
            return RedirectToAction("Details");
        }

        public int GetAllLike(int id, int Like = 0)
        {
            Idea Idea = new Idea();

            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    Idea = idea;
                }
            }

            foreach (var react in _context.Reactions)
            {
                if (Idea.IdeaId == react.IdeaId)
                {
                    if (react.React == false)
                    {
                        Like++;
                    }
                }
            }

            ViewBag.Like = Like;
            return Like;
        }

        public int GetAllDisLike(int id, int DisLike = 0)
        {
            Idea Idea = new Idea();

            foreach (var idea in _context.Ideas)
            {
                if (idea.IdeaId == id)
                {
                    Idea = idea;
                }
            }

            foreach (var react in _context.Reactions)
            {
                if (Idea.IdeaId == react.IdeaId)
                {
                    if (react.React == false)
                    {
                        DisLike++;
                    }
                }
            }
            ViewBag.DisLike = DisLike;

            return DisLike;
        }
    }
}