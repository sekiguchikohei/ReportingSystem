using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class ReportIndex
    {
        // 単数==========================================
        public ApplicationUser User { get; set; }

        // 復数==========================================
        public List<Report> Reports { get; set;} 

        public List<Attendance> Attendances { get; set;}
    }
}
