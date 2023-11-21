using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace 業務報告システム.Models
{
    //作業内容
    public class Report
    {
        //PK
        public int ReportId { get; set; }

        //提出日時
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //更新日時
        [DataType(DataType.Date)]
        public DateTime? UpDate { get; set; }

        //今日のコメント
        [MaxLength(400)]
        public string? Comment { get; set; }

        //明日の予定
        [MaxLength(200)]
        public string? TomorrowComment { get; set; }

        //User_FK
        [ForeignKey("UesrId")]
        public string UserId { get; set; }

        //User_NP
        public ApplicationUser User { get; set; }

        public Attendance Attendance { get; set; }

        //Todo_FK
        //public int TodoId { get; set; }

        //Todo_NP
    }
}
