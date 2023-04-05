using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
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
    public class VacancyServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Vacancy>> mockRepository;
        private Mock<IRepository<StringValue>> mockRepositoryStringValue;
        private Mock<IMapper> mockMapper;
        private VacancyService vacancyService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void VacancyServiceTestsInitialize()
        {
            errorMessage = "";
            mockRepository = new Mock<IRepository<Vacancy>>();
            mockRepositoryStringValue = new Mock<IRepository<StringValue>>();
            mockMapper = new Mock<IMapper>();
            vacancyService = new VacancyService(
                mockMapper.Object,
                mockRepository.Object,
                mockRepositoryStringValue.Object);
        }

        [TestCleanup()]
        public void VacancyServiceTestsCleanup()
        {
            vacancyService = null;
        }

        private List<Vacancy> GetTestVacancies()
        {
            return new List<Vacancy>() {
                new Vacancy { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 },
                new Vacancy { Id = 2, Title = "Junior JavaScrip Frontend Developer", Description = "Test description 2", Previews = 0, IsActive = true, OfficeId = 1 },
                new Vacancy { Id = 3, Title = "Senior JavaScrip Frontend Developer", Description = "Test description 3", Previews = 0, IsActive = true, OfficeId = 1 }
            };
        }

        private List<VacancyDto> GetTestVacancyDtos()
        {
            return new List<VacancyDto>()
            {
                new VacancyDto { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 2, Title = "Junior JavaScrip Frontend Developer", Description = "Test description 2", Previews = 0, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 3, Title = "Senior JavaScrip Frontend Developer", Description = "Test description 3", Previews = 0, IsActive = true, OfficeId = 1 }
            };
        }

        #endregion

        [TestMethod]
        public async Task GetVacanciesSearchResultAsync_ReturnsSearchResultWithVacancies()
        {
            //Arrange
            SearchResult<VacancyDto> searchResult = null;
            int page = 1;
            int limit = 3;
            mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ReturnsAsync(GetTestVacancies());
            mockMapper.Setup(x => x.Map<IEnumerable<VacancyDto>>(It.IsAny<IEnumerable<Vacancy>>())).Returns(GetTestVacancyDtos());

            try
            {
                // Act
                searchResult = await vacancyService.GetVacanciesSearchResultAsync(limit, page, search: "", null, null, sortField: "Id", order: OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsTrue(searchResult.ItemList.Count == limit, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(SearchResult<VacancyDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetVacancyById_ReturnsVacancyDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(GetTestVacancies().Find(c => c.Id == id));
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(GetTestVacancyDtos().Find(c => c.Id == id));

            VacancyDto vacancyDto = null;

            try
            {
                // Act
                vacancyDto = await vacancyService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(vacancyDto, errorMessage);
            Assert.IsInstanceOfType(vacancyDto, typeof(VacancyDto), errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetVacancyById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            VacancyDto vacancyDto = null;

            try
            {
                // Act
                vacancyDto = await vacancyService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(vacancyDto, errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateVacancy_ReturnsVacancyDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newVacancyDto = new VacancyDto() { Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 };
            mockMapper.Setup(x => x.Map<Vacancy>(It.IsAny<VacancyDto>())).Returns(new Vacancy());
            // pass the instance to repo, which should return model with created id:
            mockRepository.Setup(r => r.CreateAsync(new Vacancy())).ReturnsAsync(new Vacancy()
            {
                Id = int.MaxValue,
                Title = newVacancyDto.Title,
                Description = newVacancyDto.Description,
                Previews = newVacancyDto.Previews,
                IsActive = newVacancyDto.IsActive,
                OfficeId = newVacancyDto.OfficeId
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(new VacancyDto()
            {
                Id = int.MaxValue,
                Title = newVacancyDto.Title,
                Description = newVacancyDto.Description,
                Previews = newVacancyDto.Previews,
                IsActive = newVacancyDto.IsActive,
                OfficeId = newVacancyDto.OfficeId
            });

            VacancyDto createdVacancyDto = null;

            try
            {
                // Act
                createdVacancyDto = await vacancyService.CreateAsync(newVacancyDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdVacancyDto, errorMessage);
            Assert.IsInstanceOfType(createdVacancyDto, typeof(VacancyDto), errorMessage);
        }
    }
}
