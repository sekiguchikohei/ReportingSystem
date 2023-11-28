using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class ReportDetail
    {
        public ApplicationUser User { get; set; }

        public List<ApplicationUser>? Managers { get; set; }

        public List<Project> Projects { get; set; }
        public Report Report { get; set; }

        public Attendance Attendance { get; set; }

        public Feedback? Feedback { get; set; }

        
    }
}
