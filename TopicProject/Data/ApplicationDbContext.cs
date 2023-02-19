using Microsoft.EntityFrameworkCore;
using TopicProject.Models;

namespace TopicProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {

        }

        public DbSet<Topic> Topics { get; set; }
    }
}
