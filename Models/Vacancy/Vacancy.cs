using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models
{
    public class Vacancy
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title (1-50 characters) is required.")]
        [StringLength(50)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int Previews { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public IEnumerable<Candidate> Candidates { get; set; }

        [Required]
        public int OfficeId { get; set; }

        public Office Office { get; set; }
    }
}
