using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;

namespace Project_1640.Controllers
{
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TopicController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Topic> objTopicList = _db.Topics;
            return View(objTopicList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Topic obj)
        {
            if (ModelState.IsValid)
            { 
                _db.Topics.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Topic Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var topicFromDb = _db.Topics.Find(id);

            if(topicFromDb == null)
            {
                return NotFound();
            }

            return View(topicFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Topic obj)
        {
            if (ModelState.IsValid)
            {
                _db.Topics.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Topic Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var topicFromDb = _db.Topics.Find(id);

            if (topicFromDb == null)
            {
                return NotFound();
            }

            return View(topicFromDb);
        }
        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _db.Topics.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Topics.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Topic Deleted Successfully";
            return RedirectToAction("Index");
            }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _db.Topics == null)
            {
                return NotFound();
            }

            var topic = await _db.Topics.FirstOrDefaultAsync(m => m.Id == id);
            List<Idea> ideas = new List<Idea>();

            foreach (var idea in _db.Ideas)
            {
                if (id == Convert.ToInt32(idea.TopicId))
                {
                    ideas.Add(idea);
                }
            }

            IdeaTopicViewModel model = new IdeaTopicViewModel();
            model.Topics = topic;
            model.Ideas = ideas;


            return View(model);
        }

    }

}
