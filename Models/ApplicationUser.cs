using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace 業務報告システム.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public List<string> ProjectName { get; set; }

        public ICollection<Attendance> Attendances {  get; set; }
        
        public ICollection<Report> Reporst {  get; set; }

        public ICollection<Todo> Todos { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; }

    }
}
