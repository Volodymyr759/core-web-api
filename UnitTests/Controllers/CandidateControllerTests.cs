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
    public class CandidateControllerTests
    {
        #region Private members

        private string errorMessage;
        private CandidateController candidateController;
        private Mock<ICandidateService> mockCandidateService;
        private Mock<IVacancyService> mockVacancyService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CandidateControllerTestInitialize()
        {
            errorMessage = "";
            mockCandidateService = new Mock<ICandidateService>();
            mockVacancyService = new Mock<IVacancyService>();
            candidateController = new CandidateController(mockCandidateService.Object, mockVacancyService.Object);
        }

        [TestCleanup()]
        public void CandidateControllerTestsCleanup()
        {
            candidateController = null;
        }

        private CandidateDto GetTestCandidateDtoById(int id)
        {
            return new CandidateDto { Id = 1, FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "Test note 1", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 };
        }

        private IEnumerable<CandidateDto> GetTestCandidateDtos()
        {
            return new List<CandidateDto>() {
                new CandidateDto { Id = 1, FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "Test note 1", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new CandidateDto { Id = 2, FullName = "Merelin Monroe", Email = "merelin@gmail.com", Phone = "+1234567892", Notes = "Test note 2", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new CandidateDto { Id = 3, FullName = "Julia Roberts", Email = "julia@gmail.com", Phone = "+1234567893", Notes = "Test note 3", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public async Task GetById_ReturnsOkWithCandidateDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCandidateService.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(GetTestCandidateDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.GetByIdAsync(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CandidateDto), errorMessage);
            mockCandidateService.Verify(r => r.GetByIdAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCandidateService.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.GetByIdAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCandidateService.Verify(r => r.GetByIdAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedCandidateDtoByValidArg()
        {
            //Arrange
            var createCandidateDto = GetTestCandidateDtoById(1);
            mockCandidateService.Setup(r => r.CreateAsync(createCandidateDto)).ReturnsAsync(GetTestCandidateDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = await candidateController.CreateAsync(createCandidateDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CandidateDto), errorMessage);
            mockCandidateService.Verify(r => r.CreateAsync(createCandidateDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int i = 1;
            var createCandidateDto = GetTestCandidateDtoById(i);
            candidateController.ModelState.AddModelError("FullName", "Full name (1-50 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.CreateAsync(createCandidateDto) as BadRequestObjectResult;
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
        public async Task Update_ReturnsCandidateDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var candidateDtoToUpdate = GetTestCandidateDtoById(id);
            mockCandidateService.Setup(r => r.UpdateAsync(candidateDtoToUpdate)).Returns(Task.CompletedTask);
            mockCandidateService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.UpdateAsync(candidateDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(CandidateDto), errorMessage);
            mockCandidateService.Verify(r => r.UpdateAsync(candidateDtoToUpdate));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            int id = 1;
            var candidateDtoToUpdate = GetTestCandidateDtoById(id);
            mockCandidateService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.UpdateAsync(candidateDtoToUpdate) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCandidateService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int i = 1;
            var candidateDtoToUpdate = GetTestCandidateDtoById(i);
            candidateController.ModelState.AddModelError("FullName", "Full name (1-50 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.UpdateAsync(candidateDtoToUpdate) as BadRequestObjectResult;
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
            mockCandidateService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockCandidateService.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            OkResult result = null;

            try
            {
                // Act
                result = await candidateController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockCandidateService.Verify(r => r.IsExistAsync(id));
            mockCandidateService.Verify(r => r.DeleteAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockCandidateService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await candidateController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCandidateService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
