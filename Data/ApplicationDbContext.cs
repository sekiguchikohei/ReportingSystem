using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace 業務報告システム.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<業務報告システム.Models.Report> report { get; set; } = default;

        public DbSet<業務報告システム.Models.ApplicationUser>? applicationuser { get; set; }
        public DbSet<業務報告システム.Models.Attendance>? attendance { get; set; }
        public DbSet<業務報告システム.Models.Todo>? todo { get; set; }
        public DbSet<業務報告システム.Models.Feedback>? feedback { get; set; }

        public DbSet<業務報告システム.Models.Project>? project { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}