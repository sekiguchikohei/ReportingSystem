using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using 業務報告システム.Models;

namespace 業務報告システム.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<業務報告システム.Models.Report> report { get; set; } = default;

        public DbSet<業務報告システム.Models.ApplicationUser>? user { get; set; }
        public DbSet<業務報告システム.Models.Attendance>? attendance { get; set; }
        public DbSet<業務報告システム.Models.Todo>? todo { get; set; }
        public DbSet<業務報告システム.Models.Feedback>? feedback { get; set; }

        public DbSet<業務報告システム.Models.Project>? project { get; set; }

        public DbSet<業務報告システム.Models.UserProject>? userproject { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //中間テーブルを用意した場合の設定

            builder.Entity<UserProject>()
                .HasKey(up => new { up.UserId, up.ProjectId });

            builder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProjects)
                .HasForeignKey(up => up.UserId);

            builder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.ProjectId);

            //中間テーブルがない場合の設定

            //builder.Entity<Project>()
            //    .HasMany(x => x.Users)
            //    .WithMany(x => x.Projects);

        }
    }
}