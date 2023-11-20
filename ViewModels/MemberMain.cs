using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class MemberMain
    {
        // 単数==========================================

        public Report? TodayReport { get; set; }
        public Report? Report { get; set; }

        public ApplicationUser LoginMember { get; set; }

        // 複数==========================================
        public List<ApplicationUser>? Managers { get; set; }

        public List<Todo>? Todos { get; set; }

        public List<Project>? Projects { get; set; }
    }
}
