using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;
using System.Security.Claims;

namespace Project_1640.Controllers
{
    [Authorize]
    public class ReactionsController : Controller
    {
        private readonly ApplicationDbContext context;
        public readonly UserManager<IdentityUser> userManager;

        public static DateTime Topic_FinalClosureDate;
        public static string Topic_Id;

        public ReactionsController(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
        {
            context = _context;
            userManager = _userManager;
        }

        //GET Like
        public async Task<IActionResult> Like(int id, Idea idea, Reaction reaction)
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
                var userId = userManager.GetUserId(HttpContext.User);
                foreach (var ideas in context.Ideas)
                {
                    if (ideas.IdeaId == id)
                    {
                        idea = ideas;
                    }
                }
                int count = 0;
                foreach (var react in context.Reactions)
                {
                    if (idea.IdeaId == react.IdeaId && react.UserId == userId)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    Reaction Reaction = new Reaction()
                    {
                        UserId = Convert.ToString(userId),
                        IdeaId = id,
                        React = true,
                    };
                    context.Reactions.Add(Reaction);
                }
                else
                {
                    foreach (var react in context.Reactions)
                    {
                        if (idea.IdeaId == react.IdeaId && userId == react.UserId)
                        {
                            reaction = react;
                            if (react.React == false)
                            {
                                reaction.React = true;
                            }
                            else if (react.React == true)
                            {
                                reaction.React = null;
                            }
                            else
                            {
                                reaction.React = true;
                            }
                        }
                    }
                    foreach (var react in context.Reactions)
                    {
                        if (reaction.IdeaId == react.IdeaId && reaction.UserId == react.UserId)
                        {
                            react.React = reaction.React;
                            context.Reactions.Update(react);
                        }
                    }
                }
                await context.SaveChangesAsync();
                return RedirectToRoute(new { controller = "Idea", action = "Details", id });
            }
            return RedirectToRoute(new { controller = "Idea", action = "Details", id });
        }

        //GET Dislike
        public async Task<IActionResult> Dislike(int id, Idea idea, Reaction reaction)
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
                var userId = userManager.GetUserId(HttpContext.User);
                foreach (var ideas in context.Ideas)
                {
                    if (ideas.IdeaId == id)
                    {
                        idea = ideas;
                    }
                }
                int count = 0;
                foreach (var react in context.Reactions)
                {
                    if (idea.IdeaId == react.IdeaId && react.UserId == userId)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    Reaction Reaction = new Reaction()
                    {
                        UserId = Convert.ToString(userId),
                        IdeaId = id,
                        React = false,
                    };
                    context.Reactions.Add(Reaction);
                }
                else
                {
                    foreach (var react in context.Reactions)
                    {
                        if (idea.IdeaId == react.IdeaId && userId == react.UserId)
                        {
                            reaction = react;
                            if (react.React == null)
                            {
                                reaction.React = false;
                            }
                            else if (react.React == true)
                            {
                                reaction.React = false;
                            }
                            else
                            {
                                reaction.React = null;
                            }
                        }
                    }
                    foreach (var react in context.Reactions)
                    {
                        if (reaction.IdeaId == react.IdeaId && reaction.UserId == react.UserId)
                        {
                            react.React = reaction.React;
                            context.Reactions.Update(reaction);
                        }
                    }
                }
                await context.SaveChangesAsync();
                return RedirectToRoute(new { controller = "Idea", action = "Details", id });
            }
            return RedirectToRoute(new { controller = "Idea", action = "Details", id });
        }
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
    }
}