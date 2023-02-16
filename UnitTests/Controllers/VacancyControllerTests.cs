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
    public class VacancyControllerTests
    {
        #region Private Members

        private string errorMessage;
        private VacancyController vacancyController;
        private Mock<IVacancyService> mockVacancyService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void VacancyControllerTestInitialize()
        {
            errorMessage = "";
            mockVacancyService = new Mock<IVacancyService>();
            vacancyController = new VacancyController(mockVacancyService.Object);
        }

        [TestCleanup()]
        public void VacancyControllerTestsCleanup()
        {
            vacancyController = null;
        }

        private VacancyDto GetTestVacancyDtoById(int id)
        {
            return new VacancyDto { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 };
        }

        private IEnumerable<VacancyDto> GetTestVacancyDtos()
        {
            return new List<VacancyDto>() {
                new VacancyDto { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 2, Title = "Junior JavaScrip Frontend Developer", Description = "Test description 2", Previews = 0, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 3, Title = "Senior JavaScrip Frontend Developer", Description = "Test description 3", Previews = 0, IsActive = true, OfficeId = 1 }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetById_ReturnsOkWithVacancyDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockVacancyService.Setup(r => r.GetVacancyById(id)).Returns(GetTestVacancyDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = vacancyController.GetById(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(VacancyDto), errorMessage);
            mockVacancyService.Verify(r => r.GetVacancyById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockVacancyService.Setup(r => r.GetVacancyById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = vacancyController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockVacancyService.Verify(r => r.GetVacancyById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedVacancyDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var createVacancyDto = GetTestVacancyDtoById(id);
            mockVacancyService.Setup(r => r.CreateVacancy(createVacancyDto)).Returns(GetTestVacancyDtoById(id));
            CreatedResult result = null;

            try
            {
                // Act
                result = vacancyController.Create(createVacancyDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(VacancyDto), errorMessage);
            mockVacancyService.Verify(r => r.CreateVacancy(createVacancyDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createVacancyDto = GetTestVacancyDtoById(id);
            vacancyController.ModelState.AddModelError("Title", "Title (1-50 characters) is required."); // too long Title string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = vacancyController.Create(createVacancyDto) as BadRequestObjectResult;
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
        public async Task UpdateAsync_ReturnsVacancyDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var vacancyDtoToUpdate = GetTestVacancyDtoById(id);
            mockVacancyService.Setup(r => r.UpdateVacancy(vacancyDtoToUpdate)).Returns(vacancyDtoToUpdate);
            mockVacancyService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await vacancyController.UpdateAsync(vacancyDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(VacancyDto), errorMessage);
            mockVacancyService.Verify(r => r.UpdateVacancy(vacancyDtoToUpdate));
            mockVacancyService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            int id = 1;
            var vacancyDtoToUpdate = GetTestVacancyDtoById(id);
            vacancyDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await vacancyController.UpdateAsync(vacancyDtoToUpdate) as NotFoundObjectResult;
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
            int id = 1;
            var vacancyDtoToUpdate = GetTestVacancyDtoById(id);
            vacancyController.ModelState.AddModelError("Title", "Title (1-50 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await vacancyController.UpdateAsync(vacancyDtoToUpdate) as BadRequestObjectResult;
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
        public async Task DeleteAsync_ReturnsOkByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockVacancyService.Setup(r => r.DeleteVacancyAsync(id)).Returns(Task.FromResult(true));
            mockVacancyService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkResult result = null;

            try
            {
                // Act
                result = await vacancyController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockVacancyService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockVacancyService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await vacancyController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockVacancyService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}