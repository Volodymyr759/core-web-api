using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.EmployeeService;
using CoreWebApi.Services.OfficeService;
using CoreWebApi.Services.VacancyService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class OfficeServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Office>> mockOfficeRepository;
        private Mock<IRepository<Employee>> mockEmployeeRepository;
        private Mock<IRepository<Vacancy>> mockVacancyRepository;
        private Mock<IMapper> mockMapper;
        private OfficeService officeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void OfficeServiceTestsInitialize()
        {
            errorMessage = "";
            mockOfficeRepository = new Mock<IRepository<Office>>();
            mockEmployeeRepository = new Mock<IRepository<Employee>>();
            mockVacancyRepository = new Mock<IRepository<Vacancy>>();
            mockMapper = new Mock<IMapper>();
            officeService = new OfficeService(
                mockMapper.Object,
                mockOfficeRepository.Object,
                mockEmployeeRepository.Object,
                mockVacancyRepository.Object);
        }

        [TestCleanup()]
        public void OfficeServiceTestsCleanup()
        {
            officeService = null;
        }

        private List<Office> GetTestOffices()
        {
            return new List<Office>() {
                new Office { Id = 1, Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 },
                new Office { Id = 2, Name = "Dev office 1", Description = "Test description 2", Address = "Test address 2", Latitude = 1.111112m, Longitude = 2.222222m, CountryId = 1 },
                new Office { Id = 3, Name = "Dev office 2", Description = "Test description 3", Address = "Test address 3", Latitude = 1.111115m, Longitude = 2.222225m, CountryId = 1 }
            };
        }

        private List<OfficeDto> GetTestOfficeDtos()
        {
            return new List<OfficeDto>()
            {
                new OfficeDto { Id = 1, Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 },
                new OfficeDto { Id = 2, Name = "Dev office 1", Description = "Test description 2", Address = "Test address 2", Latitude = 1.111112m, Longitude = 2.222222m, CountryId = 1 },
                new OfficeDto { Id = 3, Name = "Dev office 2", Description = "Test description 3", Address = "Test address 3", Latitude = 1.111115m, Longitude = 2.222225m, CountryId = 1 }
            };
        }

        #endregion

        [TestMethod]
        public void GetAllOffices_ReturnsListOfOffices()
        {
            //Arrange
            IEnumerable<OfficeDto> officeDtos = null;
            int page = 1;
            int limit = 3;
            mockOfficeRepository.Setup(repo => repo.GetAll()).Returns(GetTestOffices());
            mockMapper.Setup(x => x.Map<IEnumerable<OfficeDto>>(It.IsAny<IEnumerable<Office>>())).Returns(GetTestOfficeDtos());

            try
            {
                // Act
                officeDtos = officeService.GetAllOffices(page, "asc", limit);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(officeDtos, errorMessage);
            Assert.IsTrue(((List<OfficeDto>)officeDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(officeDtos, typeof(IEnumerable<OfficeDto>), errorMessage);
        }

        [TestMethod]
        public void GetOfficeById_ReturnsOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingOffice = GetTestOffices().Find(c => c.Id == id);
            mockOfficeRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingOffice);
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(GetTestOfficeDtos().Find(c => c.Id == id));

            mockEmployeeRepository.Setup(e => e.GetAll()).Returns(new List<Employee>());
            mockMapper.Setup(x => x.Map<List<EmployeeDto>>(It.IsAny<List<Employee>>())).Returns(new List<EmployeeDto>());
            mockVacancyRepository.Setup(v => v.GetAll()).Returns(new List<Vacancy>());
            mockMapper.Setup(x => x.Map<List<VacancyDto>>(It.IsAny<List<Vacancy>>())).Returns(new List<VacancyDto>());

            OfficeDto officeDto = null;

            try
            {
                // Act
                officeDto = officeService.GetOfficeById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(officeDto, errorMessage);
            Assert.IsInstanceOfType(officeDto, typeof(OfficeDto), errorMessage);
            Assert.IsNotNull(officeDto.EmployeeDtos, errorMessage);
            Assert.IsNotNull(officeDto.VacancyDtos, errorMessage);
        }

        [TestMethod]
        public void GetOfficeById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockOfficeRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            OfficeDto officeDto = null;

            try
            {
                // Act
                officeDto = officeService.GetOfficeById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(officeDto, errorMessage);
        }

        [TestMethod]
        public void CreateOffice_ReturnsOfficeDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newOfficeDto = new OfficeDto() { Name = "New Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 }; ;
            mockMapper.Setup(x => x.Map<Office>(It.IsAny<OfficeDto>())).Returns(new Office());
            // pass the instance to repo, which should return model with created id:
            mockOfficeRepository.Setup(r => r.Create(new Office())).Returns(new Office()
            {
                Name = newOfficeDto.Name,
                Description = newOfficeDto.Description,
                Address = newOfficeDto.Address,
                Latitude = newOfficeDto.Latitude,
                Longitude = newOfficeDto.Longitude,
                CountryId = newOfficeDto.CountryId
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto()
            {
                Id = int.MaxValue,
                Name = newOfficeDto.Name,
                Description = newOfficeDto.Description,
                Address = newOfficeDto.Address,
                Latitude = newOfficeDto.Latitude,
                Longitude = newOfficeDto.Longitude,
                CountryId = newOfficeDto.CountryId
            });

            OfficeDto createdOfficeDto = null;

            try
            {
                // Act
                createdOfficeDto = officeService.CreateOffice(newOfficeDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdOfficeDto, errorMessage);
            Assert.IsInstanceOfType(createdOfficeDto, typeof(OfficeDto), errorMessage);
        }

        [TestMethod]
        public void UpdateOffice_ReturnsUpdatedOfficeDto()
        {
            //Arrange the same scenario like in 'Create' method
            var officeDtoToUpdate = new OfficeDto() { Id = 1, Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 };
            mockMapper.Setup(x => x.Map<Office>(It.IsAny<OfficeDto>())).Returns(new Office());
            mockOfficeRepository.Setup(r => r.Update(new Office())).Returns(new Office()
            {
                Id = int.MaxValue,
                Name = officeDtoToUpdate.Name,
                Description = officeDtoToUpdate.Description,
                Address = officeDtoToUpdate.Address,
                Latitude = officeDtoToUpdate.Latitude,
                Longitude = officeDtoToUpdate.Longitude,
                CountryId = officeDtoToUpdate.CountryId
            });
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto()
            {
                Id = int.MaxValue,
                Name = officeDtoToUpdate.Name,
                Description = officeDtoToUpdate.Description,
                Address = officeDtoToUpdate.Address,
                Latitude = officeDtoToUpdate.Latitude,
                Longitude = officeDtoToUpdate.Longitude,
                CountryId = officeDtoToUpdate.CountryId
            });

            OfficeDto updatedOfficeDto = null;

            try
            {
                // Act
                updatedOfficeDto = officeService.UpdateOffice(officeDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(updatedOfficeDto, errorMessage);
            Assert.IsInstanceOfType(updatedOfficeDto, typeof(OfficeDto), errorMessage);
        }

        [TestMethod]
        public void DeleteOfficeById_ReturnsOfficeDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockOfficeRepository.Setup(r => r.Delete(id)).Returns(GetTestOffices().Find(c => c.Id == id));
            // since repo.delete(int id) returns origin Office-object - possible to map it to dto and give it back:
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(GetTestOfficeDtos().Find(c => c.Id == id));

            OfficeDto officeDto = null;

            try
            {
                // Act
                officeDto = officeService.DeleteOffice(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(officeDto, errorMessage);
            Assert.IsInstanceOfType(officeDto, typeof(OfficeDto), errorMessage);
        }
    }
}
