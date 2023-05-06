using CoreWebApi.Controllers;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    [TestClass]
    public class CountryControllerTests
    {
        #region Private Members

        private string errorMessage;
        private CountryController countryController;
        private Mock<ICountryService> mockCountryService;
        private Mock<IOfficeService> mockOfficeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CountryControllerTestInitialize()
        {
            errorMessage = "";
            mockCountryService = new Mock<ICountryService>();
            mockOfficeService = new Mock<IOfficeService>();
            countryController = new CountryController(mockCountryService.Object, mockOfficeService.Object);
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
        public async Task GetById_ReturnsOkWithCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCountryService.Setup(r => r.GetAsync(id)).ReturnsAsync(GetTestCountryDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await countryController.GetAsync(id) as OkObjectResult;
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
            mockCountryService.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCountryService.Setup(r => r.GetAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await countryController.GetAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCountryService.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedCountryDtoByValidArg()
        {
            //Arrange
            int i = 1;
            var createCountryDto = GetTestCountryDtoById(i);
            mockCountryService.Setup(r => r.CreateAsync(createCountryDto)).ReturnsAsync(GetTestCountryDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = await countryController.CreateAsync(createCountryDto) as CreatedResult;
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
            mockCountryService.Verify(r => r.CreateAsync(createCountryDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int i = 1;
            var createCountryDto = GetTestCountryDtoById(i);
            countryController.ModelState.AddModelError("Name", "Country name (1-20 characters) is required.");// too long Name string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await countryController.CreateAsync(createCountryDto) as BadRequestObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), errorMessage);
        }

        [TestMethod]
        public async Task Update_ReturnsCountryDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var countryDtoToUpdate = GetTestCountryDtoById(id);
            mockCountryService.Setup(r => r.UpdateAsync(countryDtoToUpdate)).Returns(Task.CompletedTask);
            mockCountryService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await countryController.UpdateAsync(countryDtoToUpdate) as OkObjectResult;
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
            mockCountryService.Verify(r => r.UpdateAsync(countryDtoToUpdate));
            mockCountryService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var countryDtoToUpdate = GetTestCountryDtoById(1);
            countryDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await countryController.UpdateAsync(countryDtoToUpdate) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
        }

        [TestMethod]
        public async Task Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int i = 1;
            var countryDtoToUpdate = GetTestCountryDtoById(i);
            countryController.ModelState.AddModelError("Name", "Country name (1-20 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await countryController.UpdateAsync(countryDtoToUpdate) as BadRequestObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), errorMessage);
        }

        [TestMethod]
        public async Task Delete_ReturnsOkByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCountryService.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            mockCountryService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkResult result = null;

            try
            {
                // Act
                result = await countryController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockCountryService.Verify(r => r.IsExistAsync(id));
            mockCountryService.Verify(r => r.DeleteAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockCountryService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await countryController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCountryService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
