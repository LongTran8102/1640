using Microsoft.EntityFrameworkCore;
using Project_1640.Models;

namespace Project_1640.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Topic> Topics { get; set; }
    }
}
