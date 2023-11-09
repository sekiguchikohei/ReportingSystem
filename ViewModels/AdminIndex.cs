using 業務報告システム.Models;

namespace 業務報告システム.ViewModels
{
    public class AdminIndex
    {
        public ApplicationUser User { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<Project>? Projects { get; set; }

        public List<UserProject>? UserProjects { get; set; }

    }
}
