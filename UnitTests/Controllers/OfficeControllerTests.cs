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
    public class OfficeControllerTests
    {
        #region Private Members

        private string errorMessage;
        private OfficeController officeController;
        private Mock<IOfficeService> mockOfficeService;
        private Mock<ICountryService> mockCountryService;
        private Mock<IVacancyService> mockVacancyService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void OfficeControllerTestInitialize()
        {
            errorMessage = "";
            mockOfficeService = new Mock<IOfficeService>();
            mockCountryService = new Mock<ICountryService>();
            mockVacancyService = new Mock<IVacancyService>();
            officeController = new OfficeController(
                mockOfficeService.Object,
                mockCountryService.Object,
                mockVacancyService.Object);
        }

        [TestCleanup()]
        public void OfficeControllerTestsCleanup()
        {
            officeController = null;
        }

        private OfficeDto GetTestOfficeDtoById(int id)
        {
            return new OfficeDto { Id = 1, Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 };
        }

        private IEnumerable<OfficeDto> GetTestOfficeDtos()
        {
            return new List<OfficeDto>() {
                new OfficeDto { Id = 1, Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 },
                new OfficeDto { Id = 2,  Name = "Dev office 1", Description = "Test description 2", Address = "Test address 2", Latitude = 1.111112m, Longitude = 2.222222m, CountryId = 1 },
                new OfficeDto { Id = 3, Name = "Dev office 2", Description = "Test description 3", Address = "Test address 3", Latitude = 1.111115m, Longitude = 2.222225m, CountryId = 1 }
            };
        }

        #endregion

        #region Tests

        //[TestMethod]
        //public void GetAll_ReturnsListOfOffices()
        //{
        //    //Arrange
        //    int page = 1;
        //    string sort = "asc";
        //    int limit = 10;
        //    mockOfficeService.Setup(r => r.GetAllOffices(page, sort, limit)).Returns(GetTestOfficeDtos());
        //    OkObjectResult result = null;

        //    try
        //    {
        //        // Act
        //        result = officeController.GetAll() as OkObjectResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        errorMessage = ex.Message + " | " + ex.StackTrace;
        //    }

        //    //Assert
        //    Assert.IsNotNull(result, errorMessage);
        //    Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
        //    Assert.IsNotNull(result.Value, errorMessage);
        //    Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<OfficeDto>), errorMessage);
        //    mockOfficeService.Verify(r => r.GetAllOffices(page, sort, limit));
        //}

        [TestMethod]
        public async Task GetById_ReturnsOkWithOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockOfficeService.Setup(r => r.GetOfficeByIdAsync(id)).ReturnsAsync(GetTestOfficeDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await officeController.GetByIdAsync(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(OfficeDto), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeByIdAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockOfficeService.Setup(r => r.GetOfficeByIdAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await officeController.GetByIdAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeByIdAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedOfficeDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var createOfficeDto = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.CreateOfficeAsync(createOfficeDto)).ReturnsAsync(GetTestOfficeDtoById(id));
            CreatedResult result = null;

            try
            {
                // Act
                result = await officeController.CreateAsync(createOfficeDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(OfficeDto), errorMessage);
            mockOfficeService.Verify(r => r.CreateOfficeAsync(createOfficeDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createOfficeDto = GetTestOfficeDtoById(id);
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");// too long Name string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await officeController.CreateAsync(createOfficeDto) as BadRequestObjectResult;
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
        public async Task Update_ReturnsOfficeDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockOfficeService.Setup(r => r.UpdateOfficeAsync(officeDtoToUpdate)).Returns(Task.CompletedTask);
            OkObjectResult result = null;

            try
            {
                // Act
                result = await officeController.UpdateAsync(officeDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(OfficeDto), errorMessage);
            mockOfficeService.Verify(r => r.UpdateOfficeAsync(officeDtoToUpdate));
            mockOfficeService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await officeController.UpdateAsync(officeDtoToUpdate) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockOfficeService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await officeController.UpdateAsync(officeDtoToUpdate) as BadRequestObjectResult;
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
            mockOfficeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockOfficeService.Setup(r => r.DeleteOfficeAsync(id)).Returns(Task.CompletedTask);
            OkResult result = null;

            try
            {
                // Act
                result = await officeController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockOfficeService.Verify(r => r.IsExistAsync(id));
            mockOfficeService.Verify(r => r.DeleteOfficeAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockOfficeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await officeController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockOfficeService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
