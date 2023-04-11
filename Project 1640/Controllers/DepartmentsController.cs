using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_1640.Data;
using Project_1640.Models;
using Project_1640.ViewModels;

namespace Project_1640.Controllers
{
    [Authorize(Roles = "QA/QC Coordinator, Admin")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET Departments
        public IActionResult Index(string term = "", int currentPage = 1, string orderBy = "")
        {
            term = string.IsNullOrEmpty(term) ? "" : term.ToLower();
            var dep = new DepartmentViewModel();
            dep.NameSort = string.IsNullOrEmpty(orderBy) ? "NameDesc" : "";
            var depart = from d in _context.Department
                       where (term == "" || d.DepartmentName.ToLower().StartsWith(term))
                       select d;
            switch (orderBy)
            {
                case "NameDesc":
                    depart = depart.OrderByDescending(a => a.DepartmentName);
                    break;
                default:
                    depart = depart.OrderBy(a => a.DepartmentName);
                    break;

            }
            var totalRecords = depart.Count();
            var pageSize = 5;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            depart = depart.Skip((currentPage - 1) * pageSize).Take(pageSize);
            dep.Departments = depart;
            dep.PageSize = pageSize;
            dep.CurrentPage = currentPage;
            dep.TotalPages = totalPages;
            dep.Term = term;
            dep.OrderBy = orderBy;
            return View(dep);
        }

        //GET Create Department
        public IActionResult Create()
        {
            return View();
        }

        //POST Create Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,DepartmentName")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        //GET Edit Department
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Department == null)
            {
                return NotFound();
            }
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        //POST Edit Department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,DepartmentName")] Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        //GET Delete Department
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Department == null)
            {
                return NotFound();
            }
            var department = await _context.Department.FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        //POST Delete Department
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usedDepartment = await _context.Department
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            var count = _context.applicationUsers.Count(i => i.DepartmentId == id);
            if (count == 0)
            {
                if (_context.Department == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Department'  is null.");
            }
            var department = await _context.Department.FindAsync(id);
            if (department != null)
            {
                _context.Department.Remove(department);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.UsedDepartment = "This department is already in use";
            }
            return View(usedDepartment);

        }

        //Check department exist
        private bool DepartmentExists(int id)
        {
          return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
