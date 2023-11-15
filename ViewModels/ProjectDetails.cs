using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class ProjectDetails
    {
        public Project Project { get; set; }
        public List<ApplicationUser>? Managers { get; set; }
        public List<ApplicationUser>? Members { get; set; }

    }
}
