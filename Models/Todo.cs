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
        [Required(ErrorMessage ="タスク名を入力してください。")]
        [MaxLength(50, ErrorMessage = "タスク名は50文字以内で入力してください。")]
        public string TaskName { get; set; }

        //進捗　1～10の10段階
        [Range(0,10)]
        public int Progress { get; set; }

        //Taskの開始日
        [Required(ErrorMessage = "タスクの開始日を入力してください。")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        //Taskの終了日
        [Required(ErrorMessage = "タスクの完了日を入力してください。")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        //メンバーの備忘録
        [Required(ErrorMessage = "タスクの詳細を入力してください。")]
        [MaxLength(300, ErrorMessage = "タスクの詳細は300文字以内で入力してください。")]
        public string Comment { get; set; }

        //FK
        [ForeignKey("UesrId")]
        public string UserId { get; set; }

        //NP
        public ApplicationUser User { get; set; }

    }
}
