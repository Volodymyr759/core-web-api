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
    public class CompanyServicesControllerTests
    {
        #region Private members

        private string errorMessage;
        private CompanyServiceController companyServiceController;
        private Mock<ICompanyServiceBL> mockCompanyServiceBL;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CompanyServiceControllerTestInitialize()
        {
            errorMessage = "";
            mockCompanyServiceBL = new Mock<ICompanyServiceBL>();
            companyServiceController = new CompanyServiceController(mockCompanyServiceBL.Object);
        }

        [TestCleanup()]
        public void CompanyServiceControllerTestsCleanup()
        {
            companyServiceController = null;
        }

        private CompanyServiceDto GetTestCompanyServiceDtoById(int id)
        {
            return new CompanyServiceDto { Id = 1, Title = "Lorem Ipsum", Description = "Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi", ImageUrl = "https://somewhere.com/1", IsActive = true };
        }

        private IEnumerable<CompanyServiceDto> GetTestCompanyServiceDtos()
        {
            return new List<CompanyServiceDto>() {
                new CompanyServiceDto { Id = 1, Title ="Lorem Ipsum", Description ="Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi", ImageUrl="https://somewhere.com/1", IsActive=true },
                new CompanyServiceDto { Id = 2, Title ="Sed ut perspiciatis", Description ="Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore", ImageUrl="https://somewhere.com/2", IsActive=true },
                new CompanyServiceDto { Id = 3, Title ="Magni Dolores", Description ="Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia", ImageUrl="https://somewhere.com/3", IsActive=true },
                new CompanyServiceDto { Id = 4, Title ="Nemo Enim", Description ="At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis", ImageUrl="https://somewhere.com/4", IsActive=true }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public async Task GetById_ReturnsOkWithCompanyServiceDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCompanyServiceBL.Setup(r => r.GetAsync(id)).ReturnsAsync(GetTestCompanyServiceDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.GetAsync(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CompanyServiceDto), errorMessage);
            mockCompanyServiceBL.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCompanyServiceBL.Setup(r => r.GetAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.GetAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCompanyServiceBL.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedCompanyServiceDtoByValidArg()
        {
            //Arrange
            var createCompanyServiceDto = GetTestCompanyServiceDtoById(1);
            mockCompanyServiceBL.Setup(r => r.CreateAsync(createCompanyServiceDto)).ReturnsAsync(GetTestCompanyServiceDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = await companyServiceController.CreateAsync(createCompanyServiceDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CompanyServiceDto), errorMessage);
            mockCompanyServiceBL.Verify(r => r.CreateAsync(createCompanyServiceDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int i = 1;
            var createCompanyServiceDto = GetTestCompanyServiceDtoById(i);
            companyServiceController.ModelState.AddModelError("Title", "Title should be 1 - 100 characters");// too long Title string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.CreateAsync(createCompanyServiceDto) as BadRequestObjectResult;
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
        public async Task Update_ReturnsCompanyServiceDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(id);
            mockCompanyServiceBL.Setup(r => r.UpdateAsync(companyServiceDtoToUpdate)).Returns(Task.CompletedTask);
            mockCompanyServiceBL.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.UpdateAsync(companyServiceDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CompanyServiceDto), errorMessage);
            mockCompanyServiceBL.Verify(r => r.UpdateAsync(companyServiceDtoToUpdate));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(1);
            companyServiceDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.UpdateAsync(companyServiceDtoToUpdate) as NotFoundObjectResult;
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
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(i);
            companyServiceController.ModelState.AddModelError("Title", "Title (1 - 100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.UpdateAsync(companyServiceDtoToUpdate) as BadRequestObjectResult;
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
            mockCompanyServiceBL.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            mockCompanyServiceBL.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkResult result = null;

            try
            {
                // Act
                result = await companyServiceController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockCompanyServiceBL.Verify(r => r.IsExistAsync(id));
            mockCompanyServiceBL.Verify(r => r.DeleteAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            //mockCompanyServiceBL.Setup(r => r.GetCompanyServiceByIdAsync(id)).ReturnsAsync(value: null);
            mockCompanyServiceBL.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await companyServiceController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCompanyServiceBL.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
