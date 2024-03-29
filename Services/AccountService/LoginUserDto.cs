﻿using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool Remember { get; set; } = false;
    }
}
