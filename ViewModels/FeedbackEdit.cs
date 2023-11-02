using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class FeedbackEdit
    {
        // 単数==========================================
        public Feedback Feedback { get; set; }

        public Report Report { get; set; }

        public ApplicationUser User { get; set; }
    }
}
