using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
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

        public bool IsActive { get; set; } = true;

        public IEnumerable<CandidateDto> CandidateDtos { get; set; }

        [Required]
        public int OfficeId { get; set; }
        public OfficeDto OfficeDto { get; set; }
    }
}
