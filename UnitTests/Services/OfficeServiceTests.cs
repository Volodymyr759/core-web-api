using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using CoreWebApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    [TestClass]
    public class OfficeServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Office>> mockRepository;
        private Mock<IRepository<OfficeNameId>> mockRepositoryOfficeNameId;
        private Mock<IMapper> mockMapper;
        private OfficeService officeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void OfficeServiceTestsInitialize()
        {
            errorMessage = "";
            mockRepository = new Mock<IRepository<Office>>();
            mockRepositoryOfficeNameId = new Mock<IRepository<OfficeNameId>>();
            mockMapper = new Mock<IMapper>();
            officeService = new OfficeService(
                mockMapper.Object,
                mockRepository.Object,
                mockRepositoryOfficeNameId.Object);
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
        public async Task GetOfficesSearchResultAsync_ReturnsSearchResultWithOffices()
        {
            //Arrange
            SearchResult<OfficeDto> searchResult = null;
            int limit = 3;
            int page = 1;

            mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ReturnsAsync(GetTestOffices());
            mockMapper.Setup(x => x.Map<IEnumerable<OfficeDto>>(It.IsAny<IEnumerable<Office>>())).Returns(GetTestOfficeDtos());

            try
            {
                // Act
                searchResult = await officeService.GetOfficesSearchResultAsync(limit, page, 0);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsTrue(searchResult.ItemList.Count == limit, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(SearchResult<OfficeDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetOfficeById_ReturnsOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingOffice = GetTestOffices().Find(c => c.Id == id);
            mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(existingOffice);
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(GetTestOfficeDtos().Find(c => c.Id == id));

            OfficeDto officeDto = null;

            try
            {
                // Act
                officeDto = await officeService.GetOfficeByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(officeDto, errorMessage);
            Assert.IsInstanceOfType(officeDto, typeof(OfficeDto), errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetOfficeById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(value: null);
            OfficeDto officeDto = null;

            try
            {
                // Act
                officeDto = await officeService.GetOfficeByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(officeDto, errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateOffice_ReturnsOfficeDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newOfficeDto = new OfficeDto() { Name = "New Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 };
            mockMapper.Setup(x => x.Map<Office>(It.IsAny<OfficeDto>())).Returns(new Office());
            // pass the instance to repo, which should return model with created id:
            mockRepository.Setup(r => r.CreateAsync(new Office())).ReturnsAsync(new Office()
            {
                Id = int.MaxValue,
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
                createdOfficeDto = await officeService.CreateOfficeAsync(newOfficeDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdOfficeDto, errorMessage);
            Assert.IsInstanceOfType(createdOfficeDto, typeof(OfficeDto), errorMessage);
        }
    }
}
