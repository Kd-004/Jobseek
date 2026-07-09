using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace mainProject.Models
{
    public class Job

    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string JobDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        public string EmploymentType { get; set; }

        [Required]
        public string Experience { get; set; }

        [Required]
        public string Qualification { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Required]
        [Range(1, 10000)]
        public int Vacancy { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LastDateToApply { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Open";
    }
}