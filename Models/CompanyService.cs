using System;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Models
{
    public class CompanyService
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Image url is required.")]
        public string ImageUrl { get; set; }

        public Boolean IsActive { get; set; } = false;
    }
}
