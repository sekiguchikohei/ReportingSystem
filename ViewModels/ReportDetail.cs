using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class ReportDetail
    {
        // 単数==========================================
        public Report Report { get; set; }

        public Attendance Attendance { get; set; }

        public Feedback Feedback { get; set; }

        public ApplicationUser User { get; set; }
    }
}
