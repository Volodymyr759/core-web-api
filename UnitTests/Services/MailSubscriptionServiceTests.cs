using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.MailSubscriptionService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class MailSubscriptionServiceTests
    {
        #region Private Members

        private string errorMessage;

        private Mock<IRepository<MailSubscription>> mockMailSubscriptionRepository;

        private Mock<IMapper> mockMapper;

        private MailSubscriptionService mailSubscriptionService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriptionServiceTestsInitialize()
        {
            errorMessage = "";
            mockMailSubscriptionRepository = new Mock<IRepository<MailSubscription>>();
            mockMapper = new Mock<IMapper>();
            mailSubscriptionService = new MailSubscriptionService(mockMapper.Object, mockMailSubscriptionRepository.Object);
        }

        [TestCleanup()]
        public void MailSubscriptionServiceTestsCleanup()
        {
            mailSubscriptionService = null;
        }

        #endregion

        [TestMethod]
        public void GetAllMailSubscriptions_ReturnsListOfMailSubscriptions()
        {
            //Arrange
            IEnumerable<MailSubscriptionDto> mailSubscriptionDtos = null;
            int page = 1;
            int limit = 3;
            mockMailSubscriptionRepository.Setup(repo => repo.GetAll()).Returns(GetTestMailSubscriptions());
            mockMapper.Setup(x => x.Map<IEnumerable<MailSubscriptionDto>>(It.IsAny<IEnumerable<MailSubscription>>())).Returns(GetTestMailSubscriptionDtos());

            try
            {
                // Act
                mailSubscriptionDtos = mailSubscriptionService.GetAllMailSubscriptions(page, "asc", limit);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriptionDtos, errorMessage);
            Assert.IsTrue(((List<MailSubscriptionDto>)mailSubscriptionDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(mailSubscriptionDtos, typeof(IEnumerable<MailSubscriptionDto>), errorMessage);
        }

        [TestMethod]
        public void GetCountryById_ReturnsCountryDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingMailSubscription = ((List<MailSubscription>)GetTestMailSubscriptions()).Find(c => c.Id == id);
            mockMailSubscriptionRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingMailSubscription);
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>()))
                .Returns(((List<MailSubscriptionDto>)GetTestMailSubscriptionDtos()).Find(c => c.Id == id));
            MailSubscriptionDto mailSubscriptionDto = null;

            try
            {
                // Act
                mailSubscriptionDto = mailSubscriptionService.GetMailSubscriptionById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
        }

        [TestMethod]
        public void GetMailSubscriptionById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriptionRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            MailSubscriptionDto mailSubscriptionDto = null;

            try
            {
                // Act
                mailSubscriptionDto = mailSubscriptionService.GetMailSubscriptionById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(mailSubscriptionDto, errorMessage);
        }

        [TestMethod]
        public void CreateMailSubscription_ReturnsMailSubscriptionDto()
        {
            // Arrange 
            // scenario:
            // service recievs MailSubscriptionDto model and should map it to instance of MailSubscription-domain type;
            var newMailSubscriptionDto = new MailSubscriptionDto() { Title = "New Test subscription", Content = "Content of new test subscription" };
            mockMapper.Setup(x => x.Map<MailSubscription>(It.IsAny<MailSubscriptionDto>())).Returns(new MailSubscription());
            // pass the instance to repo, which should return MailSubscription-model with created id:
            mockMailSubscriptionRepository.Setup(r => r.Create(new MailSubscription())).Returns(new MailSubscription()
            {
                Id = int.MaxValue,
                Title = newMailSubscriptionDto.Title,
                Content = newMailSubscriptionDto.Content
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>())).Returns(new MailSubscriptionDto()
            {
                Id = int.MaxValue,
                Title = newMailSubscriptionDto.Title,
                Content = newMailSubscriptionDto.Content
            });

            MailSubscriptionDto createdMailSubscriptionDto = null;

            try
            {
                // Act
                createdMailSubscriptionDto = mailSubscriptionService.CreateMailSubscription(newMailSubscriptionDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdMailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(createdMailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
        }

        [TestMethod]
        public void UpdateMailSubscription_ReturnsUpdatedMailSubscriptionDto()
        {
            //Arrange
            // the same scenario like in 'Create' method
            var mailSubscriptionDtoToUpdate = new MailSubscriptionDto() { Id = 1, Title = "New Test subscription", Content = "Content of new test subscription" };
            mockMapper.Setup(x => x.Map<MailSubscription>(It.IsAny<MailSubscriptionDto>())).Returns(new MailSubscription());
            mockMailSubscriptionRepository.Setup(r => r.Update(new MailSubscription())).Returns(new MailSubscription()
            {
                Id = mailSubscriptionDtoToUpdate.Id,
                Title = mailSubscriptionDtoToUpdate.Title,
                Content = mailSubscriptionDtoToUpdate.Content
            });
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>())).Returns(new MailSubscriptionDto()
            {
                Id = mailSubscriptionDtoToUpdate.Id,
                Title = mailSubscriptionDtoToUpdate.Title,
                Content = mailSubscriptionDtoToUpdate.Content
            });

            MailSubscriptionDto createdMailSubscriptionDto = null;

            try
            {
                // Act
                createdMailSubscriptionDto = mailSubscriptionService.UpdateMailSubscription(mailSubscriptionDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdMailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(createdMailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
        }

        [TestMethod]
        public void DeleteMailSubscriptionById_ReturnsMailSubscriptionDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockMailSubscriptionRepository.Setup(r => r.Delete(id)).Returns(((List<MailSubscription>)GetTestMailSubscriptions()).Find(c => c.Id == id));
            // since repo.delete(int id) returns origin MailSubscription-object - possible to map it to dto object and give it back:
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>())).Returns(((List<MailSubscriptionDto>)GetTestMailSubscriptionDtos()).Find(c => c.Id == id));

            MailSubscriptionDto mailSubscriptionDto = null;

            try
            {
                // Act
                mailSubscriptionDto = mailSubscriptionService.DeleteMailSubscription(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
        }

        private IEnumerable<MailSubscription> GetTestMailSubscriptions()
        {
            return new List<MailSubscription>() {
                new MailSubscription { Id = 1, Title = "Company News", Content = "Test conyent 1" },
                new MailSubscription { Id = 2, Title = "Our Vacancies", Content = "Test conyent 2" },
                new MailSubscription { Id = 3, Title = "Other test subscription", Content = "Test conyent 3" }
            };
        }

        private IEnumerable<MailSubscriptionDto> GetTestMailSubscriptionDtos()
        {
            return new List<MailSubscriptionDto>() {
                new MailSubscriptionDto { Id = 1, Title = "Company News", Content = "Test conyent 1" },
                new MailSubscriptionDto { Id = 2, Title = "Our Vacancies", Content = "Test conyent 2" },
                new MailSubscriptionDto { Id = 3, Title = "Other test subscription", Content = "Test conyent 3" }
            };
        }
    }
}
