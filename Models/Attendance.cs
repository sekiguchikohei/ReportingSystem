using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportSystem.Models
{
    //勤怠
    public class Attendance
    {
        //PK
        public int AttendanceId { get; set; }

        //今日の日付
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        //勤怠状況
        public string Status { get; set; }

        //出社時間
        public DateTime StartTime { get; set; }

        //退社時間
        public DateTime EndTime { get; set; }

        //体調・健康状態　1～5の5段階
        [Range(1,5)]
        public int HealthRating { get; set; }

        //体調・健康状態のコメント
        [MaxLength(50)]
        public string? HealthComment { get; set; }

        //FK
        //[ForeignKey("UesrId")]
        //public string UserId { get; set; }

        ////NP
        //public ApplicationUser User { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }
    }
}
