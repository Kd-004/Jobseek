using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
 
namespace JobPortal.Models
{
    public class JobSeeker
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        // Stores the relative path/URL of the saved image
        [Display(Name = "Profile Image")]
        public string? ProfileImage { get; set; }

        // Used only for uploading a new profile image - not persisted directly
        [NotMapped]
        [Display(Name = "Upload Profile Image")]
        public IFormFile? ProfileImageFile { get; set; }

        [Display(Name = "Career Objective")]
        [DataType(DataType.MultilineText)]
        public string CareerObjective { get; set; }

        [Required]
        [Display(Name = "Highest Qualification")]
        public string HighestQualification { get; set; }

        [Display(Name = "Experience (Years)")]
        [Range(0, 50)]
        public int Experience { get; set; }

        [Display(Name = "Current Salary")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be negative")]
        public decimal? CurrentSalary { get; set; }

        [Display(Name = "Expected Salary")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Salary cannot be negative")]
        public decimal? ExpectedSalary { get; set; }

        [Display(Name = "Skills (comma separated)")]
        public string Skills { get; set; }

        // Stores the relative path/URL of the saved resume file
        [Display(Name = "Resume")]
        public string? ResumeFile { get; set; }

        // Used only for uploading a new resume - not persisted directly
        [NotMapped]
        [Display(Name = "Upload Resume")]
        public IFormFile? ResumeUpload { get; set; }

        [Display(Name = "LinkedIn Profile")]
        [Url(ErrorMessage = "Enter a valid URL")]
        public string? LinkedIn { get; set; }

        [Display(Name = "GitHub Profile")]
        [Url(ErrorMessage = "Enter a valid URL")]
        public string? GitHub { get; set; }

        [Display(Name = "Registered On")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}