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
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedMailSubscriptionDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var createMailSubscriptionDto = GetTestMailSubscriptionDtoById(id);
            mockMailSubscriptionServise.Setup(r => r.CreateMailSubscription(createMailSubscriptionDto)).Returns(GetTestMailSubscriptionDtoById(id));
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
            int id = 1;
            var createMailSubscriptionDto = GetTestMailSubscriptionDtoById(id);
            mailSubscriptionController.ModelState.AddModelError("Title", "Title should be 1 - 100 characters");// too long Title string
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Create(createMailSubscriptionDto) as BadRequestObjectResult;
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
        public void Update_ReturnsMailSubscriptionDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
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
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
            mailSubscriptionDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Update(mailSubscriptionDtoToUpdate) as NotFoundObjectResult;
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
            int id = 1;
            var mailSubscriptionDtoToUpdate = GetTestMailSubscriptionDtoById(id);
            mailSubscriptionController.ModelState.AddModelError("Title", "Title (1 - 100 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Update(mailSubscriptionDtoToUpdate) as BadRequestObjectResult;
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
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = mailSubscriptionController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriptionServise.Verify(r => r.GetMailSubscriptionById(id));
        }

        #endregion
    }
}
