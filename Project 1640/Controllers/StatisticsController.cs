using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;
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
    }
}

