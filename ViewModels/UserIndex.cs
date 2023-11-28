using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class UserIndex
    {
        public ApplicationUser User { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<Project> Projects { get; set; }
    }
}
