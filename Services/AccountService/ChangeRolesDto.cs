using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services.AccountService
{
    public class ChangeRolesDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string[] NeededRoles { get; set; }
    }
}
