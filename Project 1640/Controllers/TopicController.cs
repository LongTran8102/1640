using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities.Zlib;
using Project_1640.Data;
using Project_1640.Migrations;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Text;
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

        //Download CSV File 
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
            string[] columnName = new string[] { "IdeaId", "IdeaName", "IdeaDescription", "CreatedDate", "CategoryId", "TopicId", "FilePath, Like, Dislike" };
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
                CSV += idea.TotalLike?.ToString().Replace(",", ",") + ',';
                CSV += idea.TotalDislike?.ToString().Replace(",", ",") + ',';
                CSV += "\r\n";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(CSV);
            return File(bytes, "text/csv", "IdeasList.csv");
        }

        //Download Excel File 
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
                worksheet.Cell(currentRow, 5).Value = "Like";
                worksheet.Cell(currentRow, 6).Value = "Dislike";
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
                    worksheet.Cell(currentRow, 7).Value = idea.TotalLike;
                    worksheet.Cell(currentRow, 8).Value = idea.TotalDislike;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Topic.xlsx");
                }
            }
        }
    }
}
