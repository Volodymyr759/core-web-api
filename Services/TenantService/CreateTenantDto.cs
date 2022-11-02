using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services.TenantService
{
    public class CreateTenantDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20)]
        public string Phone { get; set; }
    }
}
