using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class AdminIndex
    {
        public ApplicationUser User { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<Project>? Projects { get; set; }

        public List<UserProject>? UserProjects { get; set; }

    }
}
