using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
