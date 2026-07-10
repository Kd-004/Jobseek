using System.ComponentModel.DataAnnotations;

namespace mainProject.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        public string  UserId { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone]
        public string Phone { get; set; }

        public string? Logo { get; set; }

        [Url(ErrorMessage = "Enter a valid website URL")]
        public string? Website { get; set; }

        [Required(ErrorMessage = "Industry is required")]
        public string Industry { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}