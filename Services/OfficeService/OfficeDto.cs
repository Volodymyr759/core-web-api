using CoreWebApi.Services.CountryService;
using CoreWebApi.Services.EmployeeService;
using CoreWebApi.Services.VacancyService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.OfficeService
{
    public class OfficeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Office name (1-100 characters) is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Office description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        [DataType("decimal(18,6)")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required.")]
        [DataType("decimal(18,6)")]
        public decimal Longitude { get; set; }

        [Required]
        public int CountryId { get; set; }
        public CountryDto CountryDto { get; set; }

        public ICollection<EmployeeDto> EmployeeDtos { get; set; }
        public ICollection<VacancyDto> VacancyDtos { get; set; }
    }
}
