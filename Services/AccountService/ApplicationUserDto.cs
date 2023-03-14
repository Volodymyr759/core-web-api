using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class ApplicationUserDto
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "UserName (1-50 characters) is required."), StringLength(50)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email (1-50 characters) is required."), StringLength(50), EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public string AvatarUrl { get; set; }
    }
}
