﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name (1-20 characters) is required.")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "International code (3 characters) is required.")]
        [StringLength(3)]
        public string Code { get; set; }

        public ICollection<OfficeDto> OfficeDtos { get; set; }
    }
}
