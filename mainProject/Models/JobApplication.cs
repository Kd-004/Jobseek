using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JobPortal.Models;

namespace mainProject.Models
{
    public class JobApplication
            { 
            [Key]
            public int Id { get; set; }

            [Required]
            public int JobId { get; set; }

            [Required]
            public int CompanyId { get; set; }

            [Required]
            public int JobSeekerId { get; set; }

            [Required]
            public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

            [Required]
            [StringLength(50)]
            public string Status { get; set; } = "Pending";

            // Navigation Properties
            [ForeignKey(nameof(JobId))]
            public virtual Job Job { get; set; }

            [ForeignKey(nameof(CompanyId))]
            public virtual Company Company { get; set; }

            [ForeignKey(nameof(JobSeekerId))]
            public virtual JobSeeker JobSeeker { get; set; }
    }
}