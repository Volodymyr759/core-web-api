using CoreWebApi.Controllers.Vacancy;
using CoreWebApi.Services.VacancyService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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
            NotFoundResult result = null;

            try
            {
                // Act
                result = vacancyController.GetById(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockVacancyService.Verify(r => r.GetVacancyById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedVacancyDtoByValidArg()
        {
            //Arrange
            var createVacancyDto = GetTestVacancyDtoById(1);
            mockVacancyService.Setup(r => r.CreateVacancy(createVacancyDto)).Returns(GetTestVacancyDtoById(1));
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
            var createVacancyDto = new VacancyDto { Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 }; // too long Title string
            vacancyController.ModelState.AddModelError("Title", "Title (1-50 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = vacancyController.Create(createVacancyDto) as BadRequestResult;
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
        public void Update_ReturnsVacancyDtoByValidArg()
        {
            //Arrange
            var vacancyDtoToUpdate = GetTestVacancyDtoById(1);
            mockVacancyService.Setup(r => r.GetVacancyById(vacancyDtoToUpdate.Id)).Returns(vacancyDtoToUpdate);
            mockVacancyService.Setup(r => r.UpdateVacancy(vacancyDtoToUpdate)).Returns(vacancyDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = vacancyController.Update(vacancyDtoToUpdate) as OkObjectResult;
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
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var vacancyDtoToUpdate = GetTestVacancyDtoById(1);
            vacancyDtoToUpdate.Id = 0; // wrong id
            NotFoundResult result = null;

            try
            {
                // Act
                result = vacancyController.Update(vacancyDtoToUpdate) as NotFoundResult;
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
            var vacancyDtoToUpdate = GetTestVacancyDtoById(1);
            vacancyDtoToUpdate.Title = "Too long Vacancy Title!!!!! Too long Vacancy Title!!!!! Too long Vacancy Title!!!!!";
            mockVacancyService.Setup(r => r.GetVacancyById(vacancyDtoToUpdate.Id)).Returns(GetTestVacancyDtoById(vacancyDtoToUpdate.Id));
            vacancyController.ModelState.AddModelError("Title", "Title (1-50 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = vacancyController.Update(vacancyDtoToUpdate) as BadRequestResult;
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
        public void Delete_ReturnsOkWithVacancyDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var vacancyToDelete = GetTestVacancyDtoById(id);
            mockVacancyService.Setup(r => r.GetVacancyById(id)).Returns(vacancyToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = vacancyController.Delete(id) as OkObjectResult;
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
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockVacancyService.Setup(r => r.GetVacancyById(id)).Returns(value: null);
            NotFoundResult result = null;

            try
            {
                // Act
                result = vacancyController.Delete(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockVacancyService.Verify(r => r.GetVacancyById(id));
        }

        #endregion
    }
}
