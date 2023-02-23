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
    public class MailSubscriptionControllerTests
    {
        #region Private Members

        private string errorMessage;
        private MailSubscriptionController mailSubscriptionController;
        private Mock<IMailSubscriptionService> mockMailSubscriptionServise;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriptionControllerTestInitialize()
        {
            errorMessage = "";
            mockMailSubscriptionServise = new Mock<IMailSubscriptionService>();
            mailSubscriptionController = new MailSubscriptionController(mockMailSubscriptionServise.Object);
        }

        [TestCleanup()]
        public void MailSubscriptionControllerTestsCleanup()
        {
            mailSubscriptionController = null;
        }

        private MailSubscriptionDto GetTestMailSubscriptionDtoById(int id)
        {
            return new MailSubscriptionDto { Id = 1, Title = "Company News", Content = "Test content 1" };
        }

        private IEnumerable<MailSubscriptionDto> GetTestMailSubscriptionDtos()
        {
            return new List<MailSubscriptionDto>() {
                new MailSubscriptionDto { Id = 1, Title = "Company News", Content = "Test content 1" },
                new MailSubscriptionDto { Id = 2, Title = "Our Vacancies", Content = "Test content 2" },
                new MailSubscriptionDto { Id = 3, Title = "Other test subscription", Content = "Test content 3" }
            };
        }

        #endregion

        #region Tests

        //[TestMethod]
        //public void GetAll_ReturnsListOfMailSubscriptions()
        //{
        //    //Arrange
        //    int page = 1;
        //    string sort = "asc";
        //    int limit = 10;
        //    mockMailSubscriptionServise.Setup(r => r.GetAllMailSubscriptions(page, sort, limit)).Returns(GetTestMailSubscriptionDtos());
        //    OkObjectResult result = null;

        //    try
        //    {
        //        // Act
        //        result = mailSubscriptionController.GetAll() as OkObjectResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        errorMessage = ex.Message + " | " + ex.StackTrace;
        //    }

        //    //Assert
        //    Assert.IsNotNull(result, errorMessage);
        //    Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
        //    Assert.IsNotNull(result.Value, errorMessage);
        //    Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<MailSubscriptionDto>), errorMessage);
        //    mockMailSubscriptionServise.Verify(r => r.GetAllMailSubscriptions(page, sort, limit));
        //}

        [TestMethod]
        public async Task GetById_ReturnsOkWithMailSubscriptionDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionByIdAsync(id)).ReturnsAsync(GetTestMailSubscriptionDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.GetByIdAsync(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriptionDto), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionByIdAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionByIdAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.GetByIdAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionByIdAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedMailSubscriptionDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var createMailSubscriptionDto = GetTestMailSubscriptionDtoById(id);
            mockMailSubscriptionServise.Setup(r => r.CreateMailSubscriptionAsync(createMailSubscriptionDto)).ReturnsAsync(GetTestMailSubscriptionDtoById(id));
            CreatedResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.CreateAsync(createMailSubscriptionDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriptionDto), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.CreateMailSubscriptionAsync(createMailSubscriptionDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createMailSubscriptionDto = GetTestMailSubscriptionDtoById(id);
            mailSubscriptionController.ModelState.AddModelError("Title", "Title should be 1 - 100 characters");// too long Title string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.CreateAsync(createMailSubscriptionDto) as BadRequestObjectResult;
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
        public async Task Update_ReturnsMailSubscriptionDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
            mockMailSubscriptionServise.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockMailSubscriptionServise.Setup(r => r.UpdateMailSubscriptionAsync(mailSubscriptionDtoToUpdate)).Returns(Task.CompletedTask);
            OkObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.UpdateAsync(mailSubscriptionDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriptionDto), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.UpdateMailSubscriptionAsync(mailSubscriptionDtoToUpdate));
            mockMailSubscriptionServise.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
            mockMailSubscriptionServise.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.UpdateAsync(mailSubscriptionDtoToUpdate) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
            mailSubscriptionController.ModelState.AddModelError("Title", "Title (1 - 100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.UpdateAsync(mailSubscriptionDtoToUpdate) as BadRequestObjectResult;
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
        public async Task Delete_ReturnsOkWithMailSubscriptionDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriptionServise.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockMailSubscriptionServise.Setup(r => r.DeleteMailSubscriptionAsync(id)).Returns(Task.CompletedTask);
            OkResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.IsExistAsync(id));
            mockMailSubscriptionServise.Verify(r => r.DeleteMailSubscriptionAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockMailSubscriptionServise.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriptionController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
