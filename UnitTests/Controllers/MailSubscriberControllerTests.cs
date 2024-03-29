﻿using CoreWebApi.Controllers;
using CoreWebApi.Library;
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
            mailSubscriberController = new MailSubscriberController(mockMailSubscriberService.Object, null, null);
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

        private SearchResult<MailSubscriberDto> GetTestMailSubscriberDtos()
        {
            var subsribers = new List<MailSubscriberDto>() {
                new MailSubscriberDto { Id = 1, Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriberDto { Id = 2, Email = "test2@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriberDto { Id = 3, Email = "test31@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 }
            };

            return new SearchResult<MailSubscriberDto>
            {
                CurrentPageNumber = 1,
                Order = OrderType.Ascending,
                PageSize = 10,
                PageCount = 1,
                SearchCriteria = "",
                TotalItemCount = 21,
                ItemList = subsribers
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public async Task GetAll_ReturnsListOfMailSubscribers()
        {
            //Arrange
            int page = 1;
            OrderType order = OrderType.Ascending;
            int limit = 10;
            mockMailSubscriberService.Setup(r => r.GetAsync(limit, page, order)).ReturnsAsync(GetTestMailSubscriberDtos());
            OkObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.GetAsync(limit, page, order) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(SearchResult<MailSubscriberDto>), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetAsync(limit, page, order));
        }

        [TestMethod]
        public async Task GetById_ReturnsOkWithMailSubscriberDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriberService.Setup(r => r.GetAsync(id)).ReturnsAsync(GetTestMailSubscriberDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.GetAsync(id) as OkObjectResult;
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
            mockMailSubscriberService.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriberService.Setup(r => r.GetAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.GetAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriberService.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task Subscribe_ReturnsCreatedMailSubscriberDtoByValidArg()
        {
            //Arrange
            var createMailSubscriberDto = GetTestMailSubscriberDtoById(1);
            mockMailSubscriberService.Setup(r => r.CreateAsync(createMailSubscriberDto)).ReturnsAsync(GetTestMailSubscriberDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.SubscribeAsync(createMailSubscriberDto) as CreatedResult;
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
            mockMailSubscriberService.Verify(r => r.CreateAsync(createMailSubscriberDto));
        }

        [TestMethod]
        public async Task Unsubscribe_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var mailSubscriberDtoToUpdate = GetTestMailSubscriberDtoById(1);
            mailSubscriberDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.UnsubscribeAsync(mailSubscriberDtoToUpdate.Id, null) as NotFoundObjectResult;
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
        public async Task Delete_ReturnsOkByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockMailSubscriberService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockMailSubscriberService.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            OkResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockMailSubscriberService.Verify(r => r.IsExistAsync(id));
            mockMailSubscriberService.Verify(r => r.DeleteAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockMailSubscriberService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await mailSubscriberController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockMailSubscriberService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
