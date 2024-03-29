﻿using Microsoft.AspNetCore.Identity;
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
        public DbSet<Category> Category { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<View> Views { get; set; }
    }
}
