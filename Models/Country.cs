using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name (1-20 characters) is required.")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "International code (3 characters) is required.")]
        [StringLength(3)]
        public string Code { get; set; }

        public ICollection<Office> Offices { get; set; }
    }
}
