using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace 業務報告システム.Models
{
    [Index(nameof(Project.Name),IsUnique = true)]
    public class Project
    {
        public int ProjectId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        //public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public ICollection<UserProject>? UserProjects { get; set; }
    }

    // 中間テーブル
    public class UserProject
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
