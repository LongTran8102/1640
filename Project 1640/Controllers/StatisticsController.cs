using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Migrations;
using Project_1640.Models;
using Project_1640.ViewModels;
using System.Linq;
using System.Security.Cryptography;

namespace Project_1640.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult IdeaDepartment()
        {
            return View();
        }

        [HttpPost]
        public List<object> IdeaDepartmentData()
        {
            List<object> data = new List<object>();

            var department = _context.Department.OrderBy(d => d.DepartmentId);
            var departmentIds = department.Select(d => d.DepartmentId).ToList();

            List<string> labels = department.Select(d => d.DepartmentName).ToList();
            data.Add(labels);

            List<int> ccounts = new List<int>();
            foreach (var departmentId in departmentIds)
            {
                var userIds = _context.applicationUsers.Where(u => u.DepartmentId == departmentId).Select(u => u.Id).ToList();

                var count = _context.Ideas.Where(i => userIds.Contains(i.UserId)).ToList().Count();
                ccounts.Add(count);
            }

            data.Add(ccounts);
            return data;
        }

        public IActionResult IdeaPerDay()
        {
            return View();
        }

        [HttpPost]
        public List<object> IdeaPerDayData()
        {
            List<object> data = new List<object>();

            List<string> days = new List<string>();
            var createdDay = _context.Ideas.OrderBy(i => i.IdeaId).Select(i => string.Format("{0}/{1}/{2}", i.CreatedDate.Day, i.CreatedDate.Month, i.CreatedDate.Year)).ToList();

            foreach (var createday in createdDay)
            {
                days.Add(createday);
            }

            var dates = days.Distinct().ToList();
            data.Add(dates);

            List<int> ccounts = new List<int>();
            List<object> strings = new List<object>();
            List<object> counts = new List<object>();

            foreach (var day in dates)
            {
                foreach (var list in createdDay)
                {
                    if (list == day)
                    {
                        strings.Add(day);
                    }
                }
            }

            foreach (var idea in dates)
            {
                foreach (var stri in strings)
                {
                    if (stri == idea)
                    {
                        counts.Add(stri);
                    }
                }
                var count = counts.Count();
                ccounts.Add(count);
                counts.RemoveAll(i => i == idea);
            }

            data.Add(ccounts);
            return data;
        }

    }
}

