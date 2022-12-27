using CoreWebApi.Services.CandidateService;
using CoreWebApi.Services.OfficeService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.VacancyService
{
    public class VacancyDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title (1-50 characters) is required.")]
        [StringLength(50)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int Previews { get; set; } = 0;

        public Boolean IsActive { get; set; } = true;

        public ICollection<CandidateDto> CandidateDtos { get; set; }

        [Required]
        public int OfficeId { get; set; }
        public OfficeDto OfficeDto { get; set; }
    }
}
