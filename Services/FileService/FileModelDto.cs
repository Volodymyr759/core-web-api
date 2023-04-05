using System;
using System.ComponentModel.DataAnnotations;

namespace CoreWebApi.Services
{
    public class FileModelDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name (1-50 characters) is required."), StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Path (1-200 characters) is required."), StringLength(200)]
        public string Path { get; set; }

        [Required(ErrorMessage = "FullPath (1-300 characters) is required."), StringLength(300)]
        public string FullPath { get; set; }

        [Required(ErrorMessage = "Thumbnail (1-300 characters) is required."), StringLength(300)]
        public string Thumbnail { get; set; }

        [Required(ErrorMessage = "Type (1-20 characters) is required."), StringLength(20)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Extention (1-10 characters) is required."), StringLength(10)]
        public string Extention { get; set; }

        [Required(ErrorMessage = "Mime (1-10 characters) is required."), StringLength(10)]
        public string Mime { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        public long Size { get; set; }

        [Required(ErrorMessage = "JoinedAt date is required.")]
        public DateTime CreatedAt { get; set; }
    }
}
