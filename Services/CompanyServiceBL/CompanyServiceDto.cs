using System;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class CompanyServiceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title (1 - 100 characters) is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image url is required.")]
        public string ImageUrl { get; set; }

        public Boolean IsActive { get; set; } = false;
    }
}
