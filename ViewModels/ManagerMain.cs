using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class ManagerMain
    {
        public ApplicationUser Manager { get; set; }
        public List<Project> Projects { get; set; }
        public List<Report>? Reports { get; set; }
        public List<Attendance>? Attendances { get; set; }
        public List<Todo>? Todos { get; set; }
        public List<ApplicationUser>? Members { get; set; }

        public List<ApplicationUser>? ReportNotSubmit {  get; set; }

        public List<Feedback>? Feedbacks { get; set; }
    }
}
