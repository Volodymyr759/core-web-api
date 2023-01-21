using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class MailSubscriberServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<MailSubscriber>> mockMailSubscriberRepository;
        private Mock<IRepository<MailSubscription>> mockMailSubscriptionRepository;
        private Mock<IMapper> mockMapper;
        private MailSubscriberService mailSubscriberService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriberServiceTestsInitialize()
        {
            errorMessage = "";
            mockMailSubscriberRepository = new Mock<IRepository<MailSubscriber>>();
            mockMailSubscriptionRepository = new Mock<IRepository<MailSubscription>>();
            mockMapper = new Mock<IMapper>();
            mailSubscriberService = new MailSubscriberService(
                mockMapper.Object,
                mockMailSubscriberRepository.Object,
                mockMailSubscriptionRepository.Object);
        }

        [TestCleanup()]
        public void CountryServiceTestsCleanup()
        {
            mailSubscriberService = null;
        }

        private List<MailSubscriber> GetTestMailSubscribers()
        {
            return new List<MailSubscriber>() {
                new MailSubscriber { Id = 1, Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriber { Id = 2, Email = "test2@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriber { Id = 3, Email = "test31@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 }
            };
        }

        private List<MailSubscriberDto> GetTestMailSubscriberDtos()
        {
            return new List<MailSubscriberDto>() {
                new MailSubscriberDto {
                    Id = 1, Email = "test1@gmail.com",
                    IsSubscribed = false,
                    MailSubscriptionId = 1,
                    MailSubscriptionDto = new MailSubscriptionDto {
                        Id = 1,
                        Title = "Test subscription",
                        Content = "Test content" } },
                new MailSubscriberDto { Id = 2, Email = "test2@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                new MailSubscriberDto { Id = 3, Email = "test3@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 }
            };
        }

        #endregion

        [TestMethod]
        public void GetAllMailSubscribers_ReturnsListOfMailSubscribers()
        {
            //Arrange
            IEnumerable<MailSubscriberDto> mailSubscriberDtos = null;
            int page = 1;
            int limit = 3;
            mockMailSubscriberRepository.Setup(repo => repo.GetAll()).Returns(GetTestMailSubscribers());
            mockMapper.Setup(x => x.Map<IEnumerable<MailSubscriberDto>>(It.IsAny<IEnumerable<MailSubscriber>>())).Returns(GetTestMailSubscriberDtos());

            try
            {
                // Act
                mailSubscriberDtos = mailSubscriberService.GetAllMailSubscribers(page, "asc", limit);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriberDtos, errorMessage);
            Assert.IsTrue(((List<MailSubscriberDto>)mailSubscriberDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(mailSubscriberDtos, typeof(IEnumerable<MailSubscriberDto>), errorMessage);
        }

        [TestMethod]
        public void GetMailSubscriberById_ReturnsMailSubscriberDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingMailSubscriber = GetTestMailSubscribers().Find(c => c.Id == id);
            mockMailSubscriberRepository.Setup(r => r.Get(id)).Returns(existingMailSubscriber);
            mockMapper.Setup(x => x.Map<MailSubscriberDto>(It.IsAny<MailSubscriber>()))
                .Returns(GetTestMailSubscriberDtos().Find(c => c.Id == id));
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>())).Returns(new MailSubscriptionDto());
            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = mailSubscriberService.GetMailSubscriberById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriberDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriberDto, typeof(MailSubscriberDto), errorMessage);
            Assert.IsNotNull(mailSubscriberDto.MailSubscriptionDto, errorMessage);
        }

        [TestMethod]
        public void GetMailSubscriberById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriberRepository.Setup(r => r.Get(id)).Returns(value: null);
            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = mailSubscriberService.GetMailSubscriberById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(mailSubscriberDto, errorMessage);
        }

        [TestMethod]
        public void Subscribe_ReturnsMailSubscriberDto()
        {
            // Arrange scenario:
            // service recievs MailSubscriberDto model and should map it to instance of MailSubscriber-domain type;
            var newMailSubscriberDto = new MailSubscriberDto() { Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 };
            mockMapper.Setup(x => x.Map<MailSubscriber>(It.IsAny<MailSubscriberDto>())).Returns(new MailSubscriber());
            // pass the instance to repo, which should return model with created id:
            mockMailSubscriberRepository.Setup(r => r.Create(new MailSubscriber())).Returns(new MailSubscriber()
            {
                Id = newMailSubscriberDto.Id,
                Email = newMailSubscriberDto.Email,
                IsSubscribed = newMailSubscriberDto.IsSubscribed,
                MailSubscriptionId = newMailSubscriberDto.Id
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<MailSubscriberDto>(It.IsAny<MailSubscriber>())).Returns(new MailSubscriberDto()
            {
                Id = newMailSubscriberDto.Id,
                Email = newMailSubscriberDto.Email,
                IsSubscribed = newMailSubscriberDto.IsSubscribed,
                MailSubscriptionId = newMailSubscriberDto.Id
            });

            MailSubscriberDto createdMailSubscriberDto = null;

            try
            {
                // Act
                createdMailSubscriberDto = mailSubscriberService.CreateMailSubscriber(newMailSubscriberDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdMailSubscriberDto, errorMessage);
            Assert.IsInstanceOfType(createdMailSubscriberDto, typeof(MailSubscriberDto), errorMessage);
        }

        [TestMethod]
        public void Unsubscribe_SetsIsSubscribedToFalse()
        {
            //Arrange
            int id = 1;// correct id
            var mailSubscriber = GetTestMailSubscribers().Find(c => c.Id == id);
            mockMailSubscriberRepository.Setup(r => r.Update(mailSubscriber)).Returns(mailSubscriber);
            mockMapper.Setup(x => x.Map<MailSubscriberDto>(It.IsAny<MailSubscriber>()))
                .Returns(GetTestMailSubscriberDtos().Find(c => c.Id == id));
            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = mailSubscriberService.UpdateMailSubscriber(GetTestMailSubscriberDtos().Find(ms => ms.Id == id));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriberDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriberDto, typeof(MailSubscriberDto), errorMessage);
            Assert.AreEqual(mailSubscriberDto.IsSubscribed, false);
        }

        [TestMethod]
        public void DeleteMailSubsriber_ReturnsMailSubscriberDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockMailSubscriberRepository.Setup(r => r.Delete(id)).Returns(GetTestMailSubscribers().Find(c => c.Id == id));
            // since repo.delete(int id) returns origin MailSubscriber-object - possible to map it to dto object and give it back:
            mockMapper.Setup(x => x.Map<MailSubscriberDto>(It.IsAny<MailSubscriber>())).Returns(((List<MailSubscriberDto>)GetTestMailSubscriberDtos()).Find(c => c.Id == id));

            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = mailSubscriberService.DeleteMailSubsriber(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriberDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriberDto, typeof(MailSubscriberDto), errorMessage);
        }
    }
}
