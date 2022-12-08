using CoreWebApi.Controllers;
using CoreWebApi.Services.TenantService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace UnitTests.Controllers
{
    [TestClass]
    public class TenantControllerTests
    {
        #region Private Members

        private TenantController tenantController;

        private string errorMessage;

        private Mock<ITenantService> mockTenantService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void TenantControllerTestInitialize()
        {
            errorMessage = "";

            mockTenantService = new Mock<ITenantService>();

            tenantController = new TenantController(mockTenantService.Object);
        }

        [TestCleanup()]
        public void TenantControllerTestsCleanup()
        {
            tenantController = null;
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetById_ReturnsOkWithTenantDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockTenantService.Setup(r => r.GetTenantById(id)).Returns(GetTestTenantDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = tenantController.GetById(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(TenantDto), errorMessage);
            mockTenantService.Verify(r => r.GetTenantById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            TenantDto tenantDto = null;
            mockTenantService.Setup(r => r.GetTenantById(id)).Returns(tenantDto);
            NotFoundResult result = null;

            try
            {
                // Act
                result = tenantController.GetById(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockTenantService.Verify(r => r.GetTenantById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedTenantDtoByValidArg()
        {
            //Arrange
            var createTenantDto = new CreateTenantDto { FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            mockTenantService.Setup(r => r.CreateTenant(createTenantDto)).Returns(GetTestTenantDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = tenantController.Create(createTenantDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(TenantDto), errorMessage);
            mockTenantService.Verify(r => r.CreateTenant(createTenantDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            var createTenantDto = new CreateTenantDto { FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123123123123111" }; // too long Phone string
            tenantController.ModelState.AddModelError("Phone", "Phone should be 0 - 20 characters");
            BadRequestResult result = null;

            try
            {
                // Act
                result = tenantController.Create(createTenantDto) as BadRequestResult;
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
        public void Update_ReturnsTenantDtoByValidArg()
        {
            //Arrange
            var tenantDtoToUpdate = new TenantDto { Id = 1, FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            mockTenantService.Setup(r => r.GetTenantById(tenantDtoToUpdate.Id)).Returns(GetTestTenantDtoById(1));
            mockTenantService.Setup(r => r.UpdateTenant(tenantDtoToUpdate)).Returns(GetTestTenantDtoById(1));
            OkObjectResult result = null;

            try
            {
                // Act
                result = tenantController.Update(tenantDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(TenantDto), errorMessage);
            mockTenantService.Verify(r => r.UpdateTenant(tenantDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var tenantDtoToUpdate = new TenantDto { Id = 0, FirstName = "", LastName = "", Email = "", Phone = "" };
            NotFoundResult result = null;

            try
            {
                // Act
                result = tenantController.Update(tenantDtoToUpdate) as NotFoundResult;
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
            var tenantDtoToUpdate = new TenantDto
            {
                Id = 1,
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "q@q.com",
                Phone = "+123123123123123123111"  // too long Phone string
            };
            mockTenantService.Setup(r => r.GetTenantById(tenantDtoToUpdate.Id)).Returns(GetTestTenantDtoById(tenantDtoToUpdate.Id));
            tenantController.ModelState.AddModelError("Phone", "Phone should be 0 - 20 characters");
            BadRequestResult result = null;

            try
            {
                // Act
                result = tenantController.Update(tenantDtoToUpdate) as BadRequestResult;
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
        public void Delete_ReturnsOkWithTenantDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var tenantToDelete = GetTestTenantDtoById(id);
            mockTenantService.Setup(r => r.GetTenantById(id)).Returns(tenantToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = tenantController.GetById(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(TenantDto), errorMessage);
            mockTenantService.Verify(r => r.GetTenantById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            TenantDto tenantDto = null;
            mockTenantService.Setup(r => r.GetTenantById(id)).Returns(tenantDto);
            NotFoundResult result = null;

            try
            {
                // Act
                result = tenantController.Delete(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockTenantService.Verify(r => r.GetTenantById(id));
        }

        #endregion

        private TenantDto GetTestTenantDtoById(int id)
        {
            var tenantDto = (id != 0) ? new TenantDto { Id = id, FirstName = "First Name", LastName = "Last Name", Email = "email@gmail.com", Phone = "+123123123" } : null;
            return tenantDto;
        }

    }
}
