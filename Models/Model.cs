using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace api.Models
{
    public class ApiContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=api.db");
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

        public UserEntity UserEntity { get; set; }
    }
}