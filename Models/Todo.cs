using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace 業務報告システム.Models
{
    //今日・明日・進捗度
    public class Todo
    {
        //PK
        public int TodoId { get; set; }

        //タスクの名前
        public string TaskName { get; set; }

        //進捗　1～10の10段階
        [Range(0,10)]
        public int Progress { get; set; }

        //Taskの開始日
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        //Taskの終了日
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        //メンバーの備忘録
        public string Comment { get; set; }

        //FK
        [ForeignKey("UesrId")]
        public string UserId { get; set; }

        //NP
        public ApplicationUser User { get; set; }

    }
}
