using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using CoreWebApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    [TestClass]
    public class MailSubscriptionServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<MailSubscription>> mockRepository;
        private Mock<IMapper> mockMapper;
        private MailSubscriptionService mailSubscriptionService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriptionServiceTestsInitialize()
        {
            errorMessage = "";
            mockRepository = new Mock<IRepository<MailSubscription>>();
            mockMapper = new Mock<IMapper>();
            mailSubscriptionService = new MailSubscriptionService(
                mockMapper.Object,
                mockRepository.Object);
        }

        [TestCleanup()]
        public void MailSubscriptionServiceTestsCleanup()
        {
            mailSubscriptionService = null;
        }

        private IEnumerable<MailSubscription> GetTestMailSubscriptions()
        {
            return new List<MailSubscription>() {
                new MailSubscription { Id = 1, Title = "Company News", Content = "Test content 1" },
                new MailSubscription { Id = 2, Title = "Our Vacancies", Content = "Test content 2" },
                new MailSubscription { Id = 3, Title = "Other test subscription", Content = "Test content 3" }
            };
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

        [TestMethod]
        public async Task GetMailSubscriptionsSearchResultAsync_ReturnsSearchResultWithMailSubscriptions()
        {
            //Arrange
            SearchResult<MailSubscriptionDto> searchResult = null;
            int page = 1;
            int limit = 3;
            mockRepository.Setup(repo => repo.GetAllAsync(null, null)).ReturnsAsync(GetTestMailSubscriptions());
            mockMapper.Setup(x => x.Map<IEnumerable<MailSubscriptionDto>>(It.IsAny<IEnumerable<MailSubscription>>())).Returns(GetTestMailSubscriptionDtos());

            try
            {
                // Act
                searchResult = await mailSubscriptionService.GetMailSubscriptionsSearchResultAsync(limit, page, order: OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsTrue(searchResult.ItemList.Count == ((List<MailSubscriptionDto>)GetTestMailSubscriptionDtos()).Count, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(SearchResult<MailSubscriptionDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetMailSubscriptionById_ReturnsMailSubscriptionDtoByCorrectId()
        {
            // Arrange
            // The method should return MailSubscription (choosed by id) with list of subscribers (if any).
            int id = 1;// correct id
            var existingMailSubscription = ((List<MailSubscription>)GetTestMailSubscriptions()).Find(c => c.Id == id);
            mockRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(existingMailSubscription);
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>()))
                .Returns(((List<MailSubscriptionDto>)GetTestMailSubscriptionDtos()).Find(c => c.Id == id));

            MailSubscriptionDto mailSubscriptionDto = null;

            try
            {
                // Act
                mailSubscriptionDto = await mailSubscriptionService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(mailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetMailSubscriptionById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            MailSubscriptionDto mailSubscriptionDto = null;

            try
            {
                // Act
                mailSubscriptionDto = await mailSubscriptionService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(mailSubscriptionDto, errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateMailSubscription_ReturnsMailSubscriptionDto()
        {
            // Arrange 
            // scenario:
            // service recievs MailSubscriptionDto model and should map it to instance of MailSubscription-domain type;
            var newMailSubscriptionDto = new MailSubscriptionDto() { Title = "New Test subscription", Content = "Content of new test subscription" };
            mockMapper.Setup(x => x.Map<MailSubscription>(It.IsAny<MailSubscriptionDto>())).Returns(new MailSubscription());
            // pass the instance to repo, which should return MailSubscription-model with created id:
            mockRepository.Setup(r => r.CreateAsync(new MailSubscription())).ReturnsAsync(new MailSubscription()
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
                createdMailSubscriptionDto = await mailSubscriptionService.CreateAsync(newMailSubscriptionDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdMailSubscriptionDto, errorMessage);
            Assert.IsInstanceOfType(createdMailSubscriptionDto, typeof(MailSubscriptionDto), errorMessage);
        }
    }
}
