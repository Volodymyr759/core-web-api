using CoreWebApi.Controllers.MailSubscriber;
using CoreWebApi.Services.MailSubscriberService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers
{
    [TestClass]
    public class MailSubscriberControllerTests
    {
        #region Private Members

        private string errorMessage;
        private MailSubscriberController mailSubscriberController;
        private Mock<IMailSubscriberService> mockMailSubscriberService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriberControllerTestInitialize()
        {
            errorMessage = "";
            mockMailSubscriberService = new Mock<IMailSubscriberService>();
            mailSubscriberController = new MailSubscriberController(mockMailSubscriberService.Object);
        }

        [TestCleanup()]
        public void MailSubscriberControllerTestsCleanup()
        {
            mockMailSubscriberService = null;
        }

        private MailSubscriberDto GetTestMailSubscriberDtoById(int id)
        {
            return new MailSubscriberDto { Id = 1, Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 };
        }

        private IEnumerable<MailSubscriberDto> GetTestMailSubscriberDtos()
        {
            return new List<MailSubscriberDto>() {
                new MailSubscriberDto { Id = 1, Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriberDto { Id = 2, Email = "test2@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriberDto { Id = 3, Email = "test31@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public void GetAll_ReturnsListOfMailSubscribers()
        {
            //Arrange
            int page = 1;
            string sort = "asc";
            int limit = 10;
            mockMailSubscriberService.Setup(r => r.GetAllMailSubscribers(page, sort, limit)).Returns(GetTestMailSubscriberDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.GetAll() as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<MailSubscriberDto>), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetAllMailSubscribers(page, sort, limit));
        }

        [TestMethod]
        public void GetById_ReturnsOkWithMailSubscriberDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriberService.Setup(r => r.GetMailSubscriberById(id)).Returns(GetTestMailSubscriberDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.GetById(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriberDto), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetMailSubscriberById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriberService.Setup(r => r.GetMailSubscriberById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetMailSubscriberById(id));
        }

        [TestMethod]
        public void Subscribe_ReturnsCreatedMailSubscriberDtoByValidArg()
        {
            //Arrange
            var createMailSubscriberDto = GetTestMailSubscriberDtoById(1);
            mockMailSubscriberService.Setup(r => r.CreateMailSubscriber(createMailSubscriberDto)).Returns(GetTestMailSubscriberDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.Subscribe(createMailSubscriberDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriberDto), errorMessage);
            mockMailSubscriberService.Verify(r => r.CreateMailSubscriber(createMailSubscriberDto));
        }

        [TestMethod]
        public void Unsubscribe_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var mailSubscriberDtoToUpdate = GetTestMailSubscriberDtoById(1);
            mailSubscriberDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.Unsubscribe(mailSubscriberDtoToUpdate.Id, null) as NotFoundObjectResult;
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
        public void Delete_ReturnsOkWithMailSubscriberDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var mailSubscriberToDelete = GetTestMailSubscriberDtoById(id);
            mockMailSubscriberService.Setup(r => r.GetMailSubscriberById(id)).Returns(mailSubscriberToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.Delete(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(MailSubscriberDto), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetMailSubscriberById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockMailSubscriberService.Setup(r => r.GetMailSubscriberById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriberController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetMailSubscriberById(id));
        }

        #endregion
    }
}
