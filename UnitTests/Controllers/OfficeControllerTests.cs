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
            NotFoundResult result = null;

            try
            {
                // Act
                result = officeController.GetById(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedOfficeDtoByValidArg()
        {
            //Arrange
            var createOfficeDto = GetTestOfficeDtoById(1);
            mockOfficeService.Setup(r => r.CreateOffice(createOfficeDto)).Returns(GetTestOfficeDtoById(1));
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
            var createOfficeDto = new OfficeDto { Name = "Main office", Description = "Test description 1", Address = "Test address 1", Latitude = 1.111111m, Longitude = 2.22222m, CountryId = 1 }; // too long Name string
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = officeController.Create(createOfficeDto) as BadRequestResult;
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
        public void Update_ReturnsOfficeDtoByValidArg()
        {
            //Arrange
            var officeDtoToUpdate = GetTestOfficeDtoById(1);
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
            var officeDtoToUpdate = GetTestOfficeDtoById(1);
            officeDtoToUpdate.Id = 0; // wrong id
            NotFoundResult result = null;

            try
            {
                // Act
                result = officeController.Update(officeDtoToUpdate) as NotFoundResult;
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
            var officeDtoToUpdate = GetTestOfficeDtoById(1);
            officeDtoToUpdate.Name = "Too long office Name!!!!! Too long office Name!!!!! Too long office Name!!!!!";
            mockOfficeService.Setup(r => r.GetOfficeById(officeDtoToUpdate.Id)).Returns(GetTestOfficeDtoById(officeDtoToUpdate.Id));
            officeController.ModelState.AddModelError("Name", "Office name (1-100 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = officeController.Update(officeDtoToUpdate) as BadRequestResult;
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
            NotFoundResult result = null;

            try
            {
                // Act
                result = officeController.Delete(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockOfficeService.Verify(r => r.GetOfficeById(id));
        }

        #endregion
    }
}
