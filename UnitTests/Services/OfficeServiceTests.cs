using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.OfficeService;
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

        private Mock<IMapper> mockMapper;

        private OfficeService officeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void OfficeServiceTestsInitialize()
        {
            errorMessage = "";
            mockOfficeRepository = new Mock<IRepository<Office>>();
            mockMapper = new Mock<IMapper>();
            officeService = new OfficeService(mockMapper.Object, mockOfficeRepository.Object);
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
                new Office { Id = 1, Name = "Dev office 1", Description = "Test description 2", Address = "Test address 2", Latitude = 1.111112m, Longitude = 2.222222m, CountryId = 1 },
                new Office { Id = 1, Name = "Dev office 2", Description = "Test description 3", Address = "Test address 3", Latitude = 1.111115m, Longitude = 2.222225m, CountryId = 1 }
            };
        }

        private List<OfficeDto> GetTestOfficeDtos()
        {
            return new List<OfficeDto>()
            {
                //new OfficeDto { Id = 1, Name = "Australia", Code = "AUS" },
                //new OfficeDto { Id = 2, Name = "Ukraine", Code = "UKR" },
                //new OfficeDto { Id = 3, Name = "UnitedStates of America", Code = "USA" }
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
            mockMapper.Setup(x => x.Map<List<OfficeDto>>(It.IsAny<List<Office>>())).Returns(GetTestOfficeDtos());

            try
            {
                // Act
                officeDtos = officeService.GetAllOffices(10, page, "", "name", "asc");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(officeDtos, errorMessage);
            Assert.IsTrue(((List<OfficeDto>)officeDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(officeDtos, typeof(List<OfficeDto>), errorMessage);
        }

        [TestMethod]
        public void GetOfficeById_ReturnsOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingOffice = GetTestOffices().Find(c => c.Id == id);
            mockOfficeRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingOffice);
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>()))
                .Returns(GetTestOfficeDtos().Find(c => c.Id == id));
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
            var newOfficeDto = new OfficeDto();
            mockMapper.Setup(x => x.Map<Office>(It.IsAny<OfficeDto>())).Returns(new Office());
            // pass the instance to repo, which should return model with created id:
            mockOfficeRepository.Setup(r => r.Create(new Office())).Returns(new Office()
            {
                //Id = int.MaxValue,
                //Name = newOfficeDto.Name,
                //Code = newOfficeDto.Code
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto()
            {
                //Id = int.MaxValue,
                //Name = newOfficeDto.Name,
                //Code = newOfficeDto.Code
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
            var officeDtoToUpdate = new OfficeDto();// { Id = 1, Name = "Bulgary", Code = "BUL" };
            mockMapper.Setup(x => x.Map<Office>(It.IsAny<OfficeDto>())).Returns(new Office());
            mockOfficeRepository.Setup(r => r.Update(new Office())).Returns(new Office()
            {
                //Id = officeDtoToUpdate.Id,
                //Name = officeDtoToUpdate.Name,
                //Code = officeDtoToUpdate.Code
            });
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto()
            {
                //Id = officeDtoToUpdate.Id,
                //Name = officeDtoToUpdate.Name,
                //Code = officeDtoToUpdate.Code
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
            // since repo.delete(int id) returns origin Office-object - possible to map it to dto object and give it back:
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
