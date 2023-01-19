using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreWebApi.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name (1-50 characters) is required."), StringLength(50)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email (1-50 characters) is required."), StringLength(50), EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Position (1-50 characters) is required."), StringLength(50)]
        public string Position { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string AvatarUrl { get; set; }

        public int? OfficeId { get; set; }

        [ForeignKey("OfficeId")]
        public virtual Office Office { get; set; }
    }
}
