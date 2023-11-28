using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class FeedbackEdit
    {
        // 単数==========================================
        public Feedback Feedback { get; set; }

        public Report Report { get; set; }

        public ApplicationUser User { get; set; }
    }
}
