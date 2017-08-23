using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiContext : IdentityDbContext<UserEntity>
    {
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=api.db");
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }

    public class UserEntity : IdentityUser
    {
        public List<Configuration> Configurations { get; set; }
    }

    public class Configuration
    {
        public int ConfigurationId { get; set; }
        public string Name { get; set; }
    }
}