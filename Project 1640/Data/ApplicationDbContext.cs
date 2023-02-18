using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_1640.Models;

namespace Project_1640.Data

{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
<<<<<<< HEAD
        public DbSet<Category> Category { get; set; }
        public DbSet<Department> Department { get; set; }
=======
        public DbSet<Project_1640.Models.Category> Category { get; set; }
>>>>>>> LongTran
    }
}
