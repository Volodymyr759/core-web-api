using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class ChangeRolesDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string[] NeededRoles { get; set; }
    }
}
