using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportSystem.Models
{
    [Index(nameof(Project.Name),IsUnique = true)]
    public class Project
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "プロジェクトの名前を入力してください。")]
        [MaxLength(30, ErrorMessage = "プロジェクトの名前は30文字以内で入力してください。")]
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
