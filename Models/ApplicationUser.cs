using Microsoft.AspNetCore.Identity;

namespace 業務報告システム.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProjectName { get; set; }
    }
}
