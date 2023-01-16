using CoreWebApi.Controllers.Office;
using CoreWebApi.Services.OfficeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers
{
    [TestClass]
    public class OfficeControllerTests
    {
        #region Private Members

        private string errorMessage;
        private OfficeController officeController;
        private Mock<IOfficeService> mockOfficeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void OfficeControllerTestInitialize()
        {
            errorMessage = "";
            mockOfficeService = new Mock<IOfficeService>();
            officeController = new OfficeController(mockOfficeService.Object);
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

        [TestMethod]
        public void GetAll_ReturnsListOfOffices()
        {
            //Arrange
            int page = 1;
            string sort = "asc";
            int limit = 10;
            mockOfficeService.Setup(r => r.GetAllOffices(page, sort, limit)).Returns(GetTestOfficeDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = officeController.GetAll() as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<OfficeDto>), errorMessage);
            mockOfficeService.Verify(r => r.GetAllOffices(page, sort, limit));
        }

        [TestMethod]
        public void GetById_ReturnsOkWithOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockOfficeService.Setup(r => r.GetOfficeById(id)).Returns(GetTestOfficeDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = officeController.GetById(id) as OkObjectResult;
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
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockOfficeService.Setup(r => r.GetOfficeById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = officeController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedOfficeDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var createOfficeDto = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.CreateOffice(createOfficeDto)).Returns(GetTestOfficeDtoById(id));
            CreatedResult result = null;

            try
            {
                // Act
                result = officeController.Create(createOfficeDto) as CreatedResult;
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
            mockOfficeService.Verify(r => r.CreateOffice(createOfficeDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createOfficeDto = GetTestOfficeDtoById(id);
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");// too long Name string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = officeController.Create(createOfficeDto) as BadRequestObjectResult;
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
        public void Update_ReturnsOfficeDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.GetOfficeById(officeDtoToUpdate.Id)).Returns(officeDtoToUpdate);
            mockOfficeService.Setup(r => r.UpdateOffice(officeDtoToUpdate)).Returns(officeDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = officeController.Update(officeDtoToUpdate) as OkObjectResult;
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
            mockOfficeService.Verify(r => r.UpdateOffice(officeDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            officeDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = officeController.Update(officeDtoToUpdate) as NotFoundObjectResult;
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
        public void Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int id = 1;
            var officeDtoToUpdate = GetTestOfficeDtoById(id);
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = officeController.Update(officeDtoToUpdate) as BadRequestObjectResult;
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
        public void Delete_ReturnsOkWithOfficeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var officeToDelete = GetTestOfficeDtoById(id);
            mockOfficeService.Setup(r => r.GetOfficeById(id)).Returns(officeToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = officeController.Delete(id) as OkObjectResult;
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
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockOfficeService.Setup(r => r.GetOfficeById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = officeController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        #endregion
    }
}
