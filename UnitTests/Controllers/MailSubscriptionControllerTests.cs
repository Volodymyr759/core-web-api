using CoreWebApi.Controllers.MailSubscription;
using CoreWebApi.Services.MailSubscriptionService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

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

        [TestMethod]
        public void GetAll_ReturnsListOfMailSubscriptions()
        {
            //Arrange
            int page = 1;
            string sort = "asc";
            int limit = 10;
            mockMailSubscriptionServise.Setup(r => r.GetAllMailSubscriptions(page, sort, limit)).Returns(GetTestMailSubscriptionDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.GetAll() as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<MailSubscriptionDto>), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetAllMailSubscriptions(page, sort, limit));
        }

        [TestMethod]
        public void GetById_ReturnsOkWithMailSubscriptionDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(id)).Returns(GetTestMailSubscriptionDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.GetById(id) as OkObjectResult;
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
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(id)).Returns(value: null);
            NotFoundResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.GetById(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedMailSubscriptionDtoByValidArg()
        {
            //Arrange
            var createMailSubscriptionDto = GetTestMailSubscriptionDtoById(1);
            mockMailSubscriptionServise.Setup(r => r.CreateMailSubscription(createMailSubscriptionDto)).Returns(GetTestMailSubscriptionDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Create(createMailSubscriptionDto) as CreatedResult;
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
            mockMailSubscriptionServise.Verify(r => r.CreateMailSubscription(createMailSubscriptionDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            var createMailSubscriptionDto = new MailSubscriptionDto { Title = "Company News", Content = "Test content 1" }; // too long Title string
            mailSubscriptionController.ModelState.AddModelError("Title", "Title should be 1 - 100 characters");
            BadRequestResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Create(createMailSubscriptionDto) as BadRequestResult;
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
        public void Update_ReturnsMailSubscriptionDtoByValidArg()
        {
            //Arrange
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(1);
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(mailSubscriptionDtoToUpdate.Id)).Returns(mailSubscriptionDtoToUpdate);
            mockMailSubscriptionServise.Setup(r => r.UpdateMailSubscription(mailSubscriptionDtoToUpdate)).Returns(mailSubscriptionDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Update(mailSubscriptionDtoToUpdate) as OkObjectResult;
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
            mockMailSubscriptionServise.Verify(r => r.UpdateMailSubscription(mailSubscriptionDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(1);
            mailSubscriptionDtoToUpdate.Id = 0; // wrong id
            NotFoundResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Update(mailSubscriptionDtoToUpdate) as NotFoundResult;
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
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(1);
            mailSubscriptionDtoToUpdate.Title = "Too long mail subscription Title!!!!! Too mail subscription service Title!!!!! Too long mail subscription Title!!!!!";
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(mailSubscriptionDtoToUpdate.Id)).Returns(GetTestMailSubscriptionDtoById(mailSubscriptionDtoToUpdate.Id));
            mailSubscriptionController.ModelState.AddModelError("Title", "Title (1 - 100 characters) is required.");
            BadRequestResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Update(mailSubscriptionDtoToUpdate) as BadRequestResult;
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
        public void Delete_ReturnsOkWithMailSubscriptionDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var mailSubscriptionToDelete = GetTestMailSubscriptionDtoById(id);
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(id)).Returns(mailSubscriptionToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Delete(id) as OkObjectResult;
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
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockMailSubscriptionServise.Setup(r => r.GetMailSubscriptionById(id)).Returns(value: null);
            NotFoundResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Delete(id) as NotFoundResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        #endregion
    }
}
