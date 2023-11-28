using ReportSystem.Models;

namespace ReportSystem.ViewModels
{
    public class ProjectDetails
    {
        public Project Project { get; set; }
        public List<ApplicationUser>? Managers { get; set; }
        public List<ApplicationUser>? Members { get; set; }

    }
}
