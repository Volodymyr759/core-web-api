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
    public class CountrySeviceTests
    {
        #region Private Members

        private string errorMessage;

        private Mock<IRepository<Country>> mockRepository;

        private Mock<IMapper> mockMapper;

        private CountryService countryService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CountryServiceTestsInitialize()
        {
            errorMessage = "";
            mockRepository = new Mock<IRepository<Country>>();
            mockMapper = new Mock<IMapper>();
            countryService = new CountryService(mockMapper.Object, mockRepository.Object);
        }

        [TestCleanup()]
        public void CountryServiceTestsCleanup()
        {
            countryService = null;
        }

        private List<Country> GetTestCountries()
        {
            return new List<Country>() {
                new Country { Id = 1, Name = "Australia", Code = "AUS" },
                new Country { Id = 2, Name = "Ukraine", Code = "UKR" },
                new Country { Id = 3, Name = "United States of America", Code = "USA" }
            };
        }

        private List<CountryDto> GetTestCountryDtos()
        {
            return new List<CountryDto>() {
                new CountryDto { Id = 1, Name = "Australia", Code = "AUS" },
                new CountryDto { Id = 2, Name = "Ukraine", Code = "UKR" },
                new CountryDto { Id = 3, Name = "United States of America", Code = "USA" }
            };
        }

        #endregion

        [TestMethod]
        public async Task GetCoutriesSearchResultAsync_ReturnsSearchResultWithCoutries()
        {
            //Arrange
            SearchResult<CountryDto> searchResult = null;
            int limit = 3;
            int page = 1;
            string sortField = "";
            OrderType order = OrderType.Ascending;
            mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ReturnsAsync(GetTestCountries());
            mockMapper.Setup(x => x.Map<IEnumerable<CountryDto>>(It.IsAny<IEnumerable<Country>>())).Returns(GetTestCountryDtos());

            try
            {
                // Act
                searchResult = await countryService.GetCountriesSearchResultAsync(limit, page, sortField, order);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsTrue(searchResult.ItemList.Count == limit, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(SearchResult<CountryDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetCountryById_ReturnsCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingCountry = GetTestCountries().Find(c => c.Id == id);
            mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(existingCountry);
            mockMapper.Setup(x => x.Map<CountryDto>(It.IsAny<Country>())).Returns(GetTestCountryDtos().Find(c => c.Id == id));
            CountryDto countryDto = null;

            try
            {
                // Act
                countryDto = await countryService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(countryDto, errorMessage);
            Assert.IsInstanceOfType(countryDto, typeof(CountryDto), errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetCountryById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            CountryDto countryDto = null;

            try
            {
                // Act
                countryDto = await countryService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(countryDto, errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateCountry_ReturnsCountryDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newCountryDto = new CountryDto() { Name = "France", Code = "FRA" };
            mockMapper.Setup(x => x.Map<Country>(It.IsAny<CountryDto>())).Returns(new Country());
            // pass the instance to repo, which should return model with created id:
            mockRepository.Setup(r => r.CreateAsync(new Country())).ReturnsAsync(new Country()
            {
                Id = int.MaxValue,
                Name = newCountryDto.Name,
                Code = newCountryDto.Code
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<CountryDto>(It.IsAny<Country>())).Returns(new CountryDto()
            {
                Id = int.MaxValue,
                Name = newCountryDto.Name,
                Code = newCountryDto.Code
            });

            CountryDto createdCountryDto = null;

            try
            {
                // Act
                createdCountryDto = await countryService.CreateAsync(newCountryDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCountryDto, errorMessage);
            Assert.IsInstanceOfType(createdCountryDto, typeof(CountryDto), errorMessage);
        }
    }
}
