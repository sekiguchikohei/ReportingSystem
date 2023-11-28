using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace ReportSystem.Models
{
    //mgrのrepoatへのFB
    public class Feedback
    {
        //PK
        public int FeedbackId { get; set; }

        //既読
        //public bool Confirm { get; set; } = false;

        //1～3の3段階評価
        [Range(1,3)]
        public int Rating { get; set; }

        //レポートに対するコメント
        public string? Comment { get; set; }

        //mgrの名前
        public string Name { get; set; }

        //FK
        public int ReportId { get; set; }

        //NP
        public Report Report { get; set; }
    }
}
