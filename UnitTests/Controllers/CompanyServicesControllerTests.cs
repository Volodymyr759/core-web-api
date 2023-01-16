using CoreWebApi.Controllers.CompanyService;
using CoreWebApi.Services.CompanyServiceBL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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
        public void GetAll_ReturnsListOfCompanyServices()
        {
            //Arrange
            int page = 1;
            string sort = "asc";
            int limit = 10;
            mockCompanyServiceBL.Setup(r => r.GetAllCompanyServices(page, sort, limit)).Returns(GetTestCompanyServiceDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.GetAll() as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<CompanyServiceDto>), errorMessage);
            mockCompanyServiceBL.Verify(r => r.GetAllCompanyServices(page, sort, limit));
        }

        [TestMethod]
        public void GetById_ReturnsOkWithCompanyServiceDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCompanyServiceBL.Setup(r => r.GetCompanyServiceById(id)).Returns(GetTestCompanyServiceDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.GetById(id) as OkObjectResult;
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
            mockCompanyServiceBL.Verify(r => r.GetCompanyServiceById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCompanyServiceBL.Setup(r => r.GetCompanyServiceById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCompanyServiceBL.Verify(r => r.GetCompanyServiceById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedCompanyServiceDtoByValidArg()
        {
            //Arrange
            var createCompanyServiceDto = GetTestCompanyServiceDtoById(1);
            mockCompanyServiceBL.Setup(r => r.CreateCompanyService(createCompanyServiceDto)).Returns(GetTestCompanyServiceDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = companyServiceController.Create(createCompanyServiceDto) as CreatedResult;
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
            mockCompanyServiceBL.Verify(r => r.CreateCompanyService(createCompanyServiceDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int i = 1;
            var createCompanyServiceDto = GetTestCompanyServiceDtoById(1);
            companyServiceController.ModelState.AddModelError("Title", "Title should be 1 - 100 characters");// too long Title string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Create(createCompanyServiceDto) as BadRequestObjectResult;
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
        public void Update_ReturnsCompanyServiceDtoByValidArg()
        {
            //Arrange
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(1);
            mockCompanyServiceBL.Setup(r => r.GetCompanyServiceById(companyServiceDtoToUpdate.Id)).Returns(companyServiceDtoToUpdate);
            mockCompanyServiceBL.Setup(r => r.UpdateCompanyService(companyServiceDtoToUpdate)).Returns(companyServiceDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Update(companyServiceDtoToUpdate) as OkObjectResult;
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
            mockCompanyServiceBL.Verify(r => r.UpdateCompanyService(companyServiceDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(1);
            companyServiceDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Update(companyServiceDtoToUpdate) as NotFoundObjectResult;
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
            int i = 1;
            var companyServiceDtoToUpdate = GetTestCompanyServiceDtoById(i);
            companyServiceController.ModelState.AddModelError("Title", "Title (1 - 100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Update(companyServiceDtoToUpdate) as BadRequestObjectResult;
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
        public void Delete_ReturnsOkWithCompanyServiceDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var companyServiceToDelete = GetTestCompanyServiceDtoById(id);
            mockCompanyServiceBL.Setup(r => r.GetCompanyServiceById(id)).Returns(companyServiceToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Delete(id) as OkObjectResult;
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
            mockCompanyServiceBL.Verify(r => r.GetCompanyServiceById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockCompanyServiceBL.Setup(r => r.GetCompanyServiceById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = companyServiceController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCompanyServiceBL.Verify(r => r.GetCompanyServiceById(id));
        }

        #endregion
    }
}
