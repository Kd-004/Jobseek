using System;
using System.ComponentModel.DataAnnotations;

namespace mainProject.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Company Name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s&.,'-]+$", ErrorMessage = "Company Name can contain only letters, numbers, spaces, &, ., comma, apostrophe and hyphen.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        public string? Logo { get; set; }

        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters.")]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Required(ErrorMessage = "Industry is required.")]
        [StringLength(100, ErrorMessage = "Industry cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s&]+$", ErrorMessage = "Industry can contain only letters, spaces and '&'.")]
        [Display(Name = "Industry")]
        public string Industry { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 500 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters.")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City can contain only letters and spaces.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "State can contain only letters and spaces.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Country can contain only letters and spaces.")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}