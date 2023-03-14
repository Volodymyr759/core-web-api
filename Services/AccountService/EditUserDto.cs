using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class EditUserDto
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20, ErrorMessage = "Maximum length should be 20 characters.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Avatar url is required.")]
        public string Avatar { get; set; }
    }
}
