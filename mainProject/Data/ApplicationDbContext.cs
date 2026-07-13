using mainProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace mainProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       // public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Job> Jobs { get; set; }
       
       
    }
}
