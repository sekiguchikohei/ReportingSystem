using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace 業務報告システム.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string? Role {  get; set; }

        //public List<Project> Projects { get; set; } = new List<Project>();

        //public List<Project>? Projects { get; set; }

        public ICollection<UserProject>? UserProjects { get; set; }

        public ICollection<Attendance> Attendances {  get; set; }
        
        public ICollection<Report> Reports {  get; set; }

        public ICollection<Todo> Todos { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; }

        }
}
