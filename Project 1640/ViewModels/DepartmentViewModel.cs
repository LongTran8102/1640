﻿using Project_1640.Models;

namespace Project_1640.ViewModels
{
    public class DepartmentViewModel
    {
        public IQueryable<Department> Departments { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string NameSort { get; set; }
        public string OrderBy { get; set; }
        public string Term { get; set; }
    }
}
