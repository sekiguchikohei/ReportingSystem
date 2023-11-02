using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class MemberMain
    {
        // 単数==========================================
        public Report Report { get; set; }

        public ApplicationUser User { get; set; }

        // 複数==========================================
        public List<Todo> Todos { get; set; }
    }
}
