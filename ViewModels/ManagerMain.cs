using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class ManagerMain
    {
        // 複数==========================================
        public List<Report> Reports { get; set; }

        public List<Attendance> Attendances { get; set; }

        public List<Todo> Todos { get; set; }

        public List<ApplicationUser>Users { get; set; }
    }
}
