using CoreWebApi.Controllers;
using CoreWebApi.Services.CountryService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers
{
    [TestClass]
    public class CountryControllerTests
    {
        #region Private Members

        private string errorMessage;
        private CountryController countryController;
        private Mock<ICountryService> mockCountryService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CountryControllerTestInitialize()
        {
            errorMessage = "";
            mockCountryService = new Mock<ICountryService>();
            countryController = new CountryController(mockCountryService.Object);
        }

        [TestCleanup()]
        public void CountryControllerTestsCleanup()
        {
            countryController = null;
        }

        private CountryDto GetTestCountryDtoById(int id)
        {
            return new CountryDto { Id = 1, Name = "Australia", Code = "AUS" };
        }

        private IEnumerable<CountryDto> GetTestCountryDtos()
        {
            return new List<CountryDto>() {
                new CountryDto { Id = 1, Name = "Australia", Code = "AUS" },
                new CountryDto { Id = 2, Name = "Ukraine", Code = "UKR" },
                new CountryDto { Id = 3, Name = "United States of America", Code = "USA" }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetAll_ReturnsListOfCountries()
        {
            //Arrange
            int page = 1;
            string sort = "asc";
            int limit = 10;
            mockCountryService.Setup(r => r.GetAllCountries(page, sort, limit)).Returns(GetTestCountryDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = countryController.GetAll() as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<CountryDto>), errorMessage);
            mockCountryService.Verify(r => r.GetAllCountries(page, sort, limit));
        }

        [TestMethod]
        public void GetById_ReturnsOkWithCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCountryService.Setup(r => r.GetCountryById(id)).Returns(GetTestCountryDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = countryController.GetById(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CountryDto), errorMessage);
            mockCountryService.Verify(r => r.GetCountryById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCountryService.Setup(r => r.GetCountryById(id)).Returns(value: null);
            NotFoundResult result = null;

            try
            {
                // Act
                result = countryController.GetById(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockCountryService.Verify(r => r.GetCountryById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedCountryDtoByValidArg()
        {
            //Arrange
            var createCountryDto = GetTestCountryDtoById(1);
            mockCountryService.Setup(r => r.CreateCountry(createCountryDto)).Returns(GetTestCountryDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = countryController.Create(createCountryDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CountryDto), errorMessage);
            mockCountryService.Verify(r => r.CreateCountry(createCountryDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            var createCountryDto = new CountryDto { Name = "Australia", Code = "AUS" }; // too long Name string
            countryController.ModelState.AddModelError("Name", "Country name (1-20 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = countryController.Create(createCountryDto) as BadRequestResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult), errorMessage);
        }

        [TestMethod]
        public void Update_ReturnsCountryDtoByValidArg()
        {
            //Arrange
            var countryDtoToUpdate = GetTestCountryDtoById(1);
            mockCountryService.Setup(r => r.GetCountryById(countryDtoToUpdate.Id)).Returns(countryDtoToUpdate);
            mockCountryService.Setup(r => r.UpdateCountry(countryDtoToUpdate)).Returns(countryDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = countryController.Update(countryDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CountryDto), errorMessage);
            mockCountryService.Verify(r => r.UpdateCountry(countryDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var countryDtoToUpdate = GetTestCountryDtoById(1);
            countryDtoToUpdate.Id = 0; // wrong id
            NotFoundResult result = null;

            try
            {
                // Act
                result = countryController.Update(countryDtoToUpdate) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
        }

        [TestMethod]
        public void Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            var countryDtoToUpdate = GetTestCountryDtoById(1);
            countryDtoToUpdate.Name = "Too long conutry Name!!!!! Too long conutry Name!!!!! Too long conutry Name!!!!! Too long conutry Name!!!!!";
            mockCountryService.Setup(r => r.GetCountryById(countryDtoToUpdate.Id)).Returns(GetTestCountryDtoById(countryDtoToUpdate.Id));
            countryController.ModelState.AddModelError("Name", "Country name (1-20 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = countryController.Update(countryDtoToUpdate) as BadRequestResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestResult), errorMessage);
        }

        [TestMethod]
        public void Delete_ReturnsOkWithCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var countryToDelete = GetTestCountryDtoById(id);
            mockCountryService.Setup(r => r.GetCountryById(id)).Returns(countryToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = countryController.Delete(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CountryDto), errorMessage);
            mockCountryService.Verify(r => r.GetCountryById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockCountryService.Setup(r => r.GetCountryById(id)).Returns(value: null);
            NotFoundResult result = null;

            try
            {
                // Act
                result = countryController.Delete(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockCountryService.Verify(r => r.GetCountryById(id));
        }

        #endregion
    }
}
