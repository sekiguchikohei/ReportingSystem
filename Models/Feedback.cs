using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace 業務報告システム.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public bool Confirm { get; set; } = false;

        [Range(1,3)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public string Name { get; set; }

        public int ReportId { get; set; }

        public Report Report { get; set; }
    }
}
