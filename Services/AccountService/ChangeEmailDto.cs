using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Existing Email")]
        public string ExistingEmail { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
