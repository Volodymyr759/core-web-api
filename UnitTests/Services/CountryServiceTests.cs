using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.CountryService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class CountrySeviceTests
    {
        #region Private Members

        private string errorMessage;

        private Mock<IRepository<Country>> mockCountryRepository;

        private Mock<IMapper> mockMapper;

        private CountryService countryService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CountryServiceTestsInitialize()
        {
            errorMessage = "";
            mockCountryRepository = new Mock<IRepository<Country>>();
            mockMapper = new Mock<IMapper>();
            countryService = new CountryService(mockMapper.Object, mockCountryRepository.Object);
        }

        [TestCleanup()]
        public void CountryServiceTestsCleanup()
        {
            countryService = null;
        }

        private IEnumerable<Country> GetTestCountries()
        {
            return new List<Country>() {
                new Country { Id = 1, Name = "Australia", Code = "AUS" },
                new Country { Id = 2, Name = "Ukraine", Code = "UKR" },
                new Country { Id = 3, Name = "UnitedStates of America", Code = "USA" }
            };
        }

        private IEnumerable<CountryDto> GetTestCountryDtos()
        {
            return new List<CountryDto>() {
                new CountryDto { Id = 1, Name = "Australia", Code = "AUS" },
                new CountryDto { Id = 2, Name = "Ukraine", Code = "UKR" },
                new CountryDto { Id = 3, Name = "UnitedStates of America", Code = "USA" }
            };
        }

        #endregion

        [TestMethod]
        public void GetAllCountries_ReturnsListOfCountries()
        {
            //Arrange
            IEnumerable<CountryDto> countryDtos = null;
            int page = 1;
            int limit = 3;
            mockCountryRepository.Setup(repo => repo.GetAll()).Returns(GetTestCountries());
            mockMapper.Setup(x => x.Map<IEnumerable<CountryDto>>(It.IsAny<IEnumerable<Country>>())).Returns(GetTestCountryDtos());

            try
            {
                // Act
                countryDtos = countryService.GetAllCountries(page, "asc", limit);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(countryDtos, errorMessage);
            Assert.IsTrue(((List<CountryDto>)countryDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(countryDtos, typeof(IEnumerable<CountryDto>), errorMessage);
        }

        [TestMethod]
        public void GetCountryById_ReturnsCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingCountry = ((List<Country>)GetTestCountries()).Find(c => c.Id == id);
            mockCountryRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingCountry);
            mockMapper.Setup(x => x.Map<CountryDto>(It.IsAny<Country>()))
                .Returns(((List<CountryDto>)GetTestCountryDtos()).Find(c => c.Id == id));
            CountryDto countryDto = null;

            try
            {
                // Act
                countryDto = countryService.GetCountryById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(countryDto, errorMessage);
            Assert.IsInstanceOfType(countryDto, typeof(CountryDto), errorMessage);
        }

        [TestMethod]
        public void GetCountryById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCountryRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            CountryDto countryDto = null;

            try
            {
                // Act
                countryDto = countryService.GetCountryById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(countryDto, errorMessage);
        }

        [TestMethod]
        public void CreateCountry_ReturnsCountryDto()
        {
            // Arrange 
            // scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newCountryDto = new CountryDto() { Name = "France", Code = "FRA" };
            mockMapper.Setup(x => x.Map<Country>(It.IsAny<CountryDto>())).Returns(new Country());
            // pass the instance to repo, which should return model with created id:
            mockCountryRepository.Setup(r => r.Create(new Country())).Returns(new Country()
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
                createdCountryDto = countryService.CreateCountry(newCountryDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCountryDto, errorMessage);
            Assert.IsInstanceOfType(createdCountryDto, typeof(CountryDto), errorMessage);
        }

        [TestMethod]
        public void UpdateCountry_ReturnsUpdatedCountryDto()
        {
            //Arrange
            // the same scenario like in 'Create' method
            var countryDtoToUpdate = new CountryDto() { Id = 1, Name = "Bulgary", Code = "BUL" };
            mockMapper.Setup(x => x.Map<Country>(It.IsAny<CountryDto>())).Returns(new Country());
            mockCountryRepository.Setup(r => r.Update(new Country())).Returns(new Country()
            {
                Id = countryDtoToUpdate.Id,
                Name = countryDtoToUpdate.Name,
                Code = countryDtoToUpdate.Code
            });
            mockMapper.Setup(x => x.Map<CountryDto>(It.IsAny<Country>())).Returns(new CountryDto()
            {
                Id = countryDtoToUpdate.Id,
                Name = countryDtoToUpdate.Name,
                Code = countryDtoToUpdate.Code
            });

            CountryDto createdCountryDto = null;

            try
            {
                // Act
                createdCountryDto = countryService.UpdateCountry(countryDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCountryDto, errorMessage);
            Assert.IsInstanceOfType(createdCountryDto, typeof(CountryDto), errorMessage);
        }

        [TestMethod]
        public void DeleteCountryById_ReturnsCountryDto()
        {
            // Arrange
            // scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockCountryRepository.Setup(r => r.Delete(id)).Returns(((List<Country>)GetTestCountries()).Find(c => c.Id == id));
            // since repo.delete(int id) returns origin Country-object - possible to map it to dto object and give it back:
            mockMapper.Setup(x => x.Map<CountryDto>(It.IsAny<Country>())).Returns(((List<CountryDto>)GetTestCountryDtos()).Find(c => c.Id == id));

            CountryDto countryDto = null;

            try
            {
                // Act
                countryDto = countryService.DeleteCountry(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(countryDto, errorMessage);
            Assert.IsInstanceOfType(countryDto, typeof(CountryDto), errorMessage);
        }
    }
}
