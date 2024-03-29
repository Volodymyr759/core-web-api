﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class CandidateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name (1-50 characters) is required."), StringLength(50)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email (1-50 characters) is required."), StringLength(50), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number (1-15 characters) is required."), StringLength(15)]
        public string Phone { get; set; }

        public string Notes { get; set; }

        public bool IsDismissed { get; set; } = false;

        [Required(ErrorMessage = "JoinedAt date is required.")]
        public DateTime JoinedAt { get; set; }

        [Required]
        public int VacancyId { get; set; }

        public VacancyDto VacancyDto { get; set; }
    }
}
