using CoreWebApi.Controllers.Candidate;
using CoreWebApi.Services.CandidateService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers
{
    [TestClass]
    public class CandidateControllerTests
    {
        #region Private members

        private string errorMessage;
        private CandidateController candidateController;
        private Mock<ICandidateService> mockCandidateService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CandidateControllerTestInitialize()
        {
            errorMessage = "";
            mockCandidateService = new Mock<ICandidateService>();
            candidateController = new CandidateController(mockCandidateService.Object);
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
        public void GetById_ReturnsOkWithCandidateDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockCandidateService.Setup(r => r.GetCandidateById(id)).Returns(GetTestCandidateDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = candidateController.GetById(id) as OkObjectResult;
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
            mockCandidateService.Verify(r => r.GetCandidateById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCandidateService.Setup(r => r.GetCandidateById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = candidateController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCandidateService.Verify(r => r.GetCandidateById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedCandidateDtoByValidArg()
        {
            //Arrange
            var createCandidateDto = GetTestCandidateDtoById(1);
            mockCandidateService.Setup(r => r.CreateCandidate(createCandidateDto)).Returns(GetTestCandidateDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = candidateController.Create(createCandidateDto) as CreatedResult;
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
            mockCandidateService.Verify(r => r.CreateCandidate(createCandidateDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int i = 1;
            var createCandidateDto = GetTestCandidateDtoById(i);
            candidateController.ModelState.AddModelError("FullName", "Full name (1-50 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Create(createCandidateDto) as BadRequestObjectResult;
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
        public void Update_ReturnsCandidateDtoByValidArg()
        {
            //Arrange
            var candidateDtoToUpdate = GetTestCandidateDtoById(1);
            mockCandidateService.Setup(r => r.GetCandidateById(candidateDtoToUpdate.Id)).Returns(candidateDtoToUpdate);
            mockCandidateService.Setup(r => r.UpdateCandidate(candidateDtoToUpdate)).Returns(candidateDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Update(candidateDtoToUpdate) as OkObjectResult;
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
            mockCandidateService.Verify(r => r.UpdateCandidate(candidateDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var candidateDtoToUpdate = GetTestCandidateDtoById(1);
            candidateDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Update(candidateDtoToUpdate) as NotFoundObjectResult;
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
            var candidateDtoToUpdate = GetTestCandidateDtoById(i);
            candidateController.ModelState.AddModelError("FullName", "Full name (1-50 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Update(candidateDtoToUpdate) as BadRequestObjectResult;
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
        public void Delete_ReturnsOkWithCandidateDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var candidateToDelete = GetTestCandidateDtoById(id);
            mockCandidateService.Setup(r => r.GetCandidateById(id)).Returns(candidateToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Delete(id) as OkObjectResult;
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
            mockCandidateService.Verify(r => r.GetCandidateById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockCandidateService.Setup(r => r.GetCandidateById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = candidateController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockCandidateService.Verify(r => r.GetCandidateById(id));
        }

        #endregion
    }
}
