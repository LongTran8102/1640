using DocumentFormat.OpenXml.InkML;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.Zlib;
using Project_1640.Data;
using Project_1640.Migrations;
using Project_1640.Models;
using Project_1640.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Project_1640.Controllers
{
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext context;
        private IHostingEnvironment oIHostingEnvironment;

        public TopicController(ApplicationDbContext _context, IHostingEnvironment _oIHostingEnvironment)
        {
            oIHostingEnvironment = _oIHostingEnvironment;
            context = _context;
        }

        public IActionResult Index()
        {
            IEnumerable<Topic> objTopicList = context.Topics;
            return View(objTopicList);
        }

        //GET: Create Topic
        public IActionResult Create()
        {
            return View();
        }

        //POST: Create Topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Topic obj)
        {
            if (ModelState.IsValid)
            {
                context.Topics.Add(obj);
                context.SaveChanges();
                TempData["success"] = "Topic Created Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET: Edit Topic
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var topicFromDb = context.Topics.Find(id);
            if(topicFromDb == null)
            {
                return NotFound();
            }
            return View(topicFromDb);
        }

        //POST: Edit Topic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Topic obj)
        {
            if (ModelState.IsValid)
            {
                context.Topics.Update(obj);
                context.SaveChanges();
                TempData["success"] = "Topic Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET Delete Topic
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var topicFromDb = context.Topics.Find(id);
            if (topicFromDb == null)
            {
                return NotFound();
            }
            return View(topicFromDb);
        }

        //POST Delete Topic
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = context.Topics.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            context.Topics.Remove(obj);
            context.SaveChanges();
            TempData["success"] = "Topic Deleted Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == null || context.Topics == null)
            {
                return NotFound();
            }
            var topic = await context.Topics.FirstOrDefaultAsync(m => m.Id == id);
            List<Idea> ideaList = new List<Idea>();
            foreach (var ideas in context.Ideas)
            {
                if (id == Convert.ToInt32(ideas.TopicId))
                {
                    ideaList.Add(ideas);
                }
            }
            IdeaTopicViewModel model = new IdeaTopicViewModel();
            model.Topics = topic;
            model.Ideas = ideaList;
            return View(model);
        }

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
                    if (Convert.ToInt32(idea.TopicId) == id)
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
    }
}
