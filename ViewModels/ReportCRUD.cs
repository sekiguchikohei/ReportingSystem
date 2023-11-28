using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class ReportCRUD
    {
        // 単数==========================================
        public Report Report { get; set; }

        public Attendance Attendance { get; set; }

        public ApplicationUser User { get; set; }
    }
}
