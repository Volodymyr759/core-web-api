using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
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
    public class MailSubscriberServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<MailSubscriber>> mockMailSubscriberRepository;
        private Mock<IMapper> mockMapper;
        private Mock<ISearchResult<MailSubscriberDto>> mockSearchResult;
        private Mock<IServiceResult<MailSubscriber>> mockServiceResult;
        private MailSubscriberService mailSubscriberService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void MailSubscriberServiceTestsInitialize()
        {
            errorMessage = "";
            mockMailSubscriberRepository = new Mock<IRepository<MailSubscriber>>();
            mockSearchResult = new Mock<ISearchResult<MailSubscriberDto>>();
            mockServiceResult = new Mock<IServiceResult<MailSubscriber>>();
            mockMapper = new Mock<IMapper>();
            mailSubscriberService = new MailSubscriberService(
                mockMapper.Object,
                mockMailSubscriberRepository.Object,
                mockSearchResult.Object,
                mockServiceResult.Object);
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

        private List<MailSubscriberDto> GetListTestMailSubscriberDtos()
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

        private ServiceResult<MailSubscriber> GetSubscribersServiceResult()
        {
            return new ServiceResult<MailSubscriber>()
            {
                TotalCount = 3,
                Items = new List<MailSubscriber>()
                {
                    new MailSubscriber { Id = 1, Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                    new MailSubscriber { Id = 2, Email = "test2@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 },
                    new MailSubscriber { Id = 3, Email = "test31@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 }
                }
            };
        }

        #endregion

        [TestMethod]
        public async Task GetAsync_ReturnsListOfMailSubscribers()
        {
            //Arrange
            ISearchResult<MailSubscriberDto> mailSubscriberDtos = null;
            int page = 1;
            int limit = 3;
            mockMailSubscriberRepository.Setup(repo => repo.GetAsync(limit, page, null, null, null)).ReturnsAsync(GetSubscribersServiceResult());
            mockMapper.Setup(x => x.Map<IEnumerable<MailSubscriberDto>>(It.IsAny<IEnumerable<MailSubscriber>>())).Returns(GetListTestMailSubscriberDtos());

            try
            {
                // Act
                mailSubscriberDtos = await mailSubscriberService.GetAsync(limit, page, OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(mailSubscriberDtos, errorMessage);
            Assert.IsInstanceOfType(mailSubscriberDtos, typeof(ISearchResult<MailSubscriberDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetMailSubscriberById_ReturnsMailSubscriberDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingMailSubscriber = GetTestMailSubscribers().Find(c => c.Id == id);
            mockMailSubscriberRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(existingMailSubscriber);
            mockMapper.Setup(x => x.Map<MailSubscriberDto>(It.IsAny<MailSubscriber>()))
                .Returns(GetListTestMailSubscriberDtos().Find(c => c.Id == id));
            mockMapper.Setup(x => x.Map<MailSubscriptionDto>(It.IsAny<MailSubscription>())).Returns(new MailSubscriptionDto());
            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = await mailSubscriberService.GetAsync(id);
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
        public async Task GetMailSubscriberById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockMailSubscriberRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            MailSubscriberDto mailSubscriberDto = null;

            try
            {
                // Act
                mailSubscriberDto = await mailSubscriberService.GetAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(mailSubscriberDto, errorMessage);
        }

        [TestMethod]
        public async Task Subscribe_ReturnsMailSubscriberDto()
        {
            // Arrange scenario:
            // service recievs MailSubscriberDto model and should map it to instance of MailSubscriber-domain type;
            var newMailSubscriberDto = new MailSubscriberDto() { Email = "test1@gmail.com", IsSubscribed = true, MailSubscriptionId = 1 };
            mockMapper.Setup(x => x.Map<MailSubscriber>(It.IsAny<MailSubscriberDto>())).Returns(new MailSubscriber());
            // pass the instance to repo, which should return model with created id:
            mockMailSubscriberRepository.Setup(r => r.CreateAsync(new MailSubscriber())).ReturnsAsync(new MailSubscriber()
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
                createdMailSubscriberDto = await mailSubscriberService.CreateAsync(newMailSubscriberDto);
                createdMailSubscriberDto = await mailSubscriberService.CreateAsync(newMailSubscriberDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdMailSubscriberDto, errorMessage);
            Assert.IsInstanceOfType(createdMailSubscriberDto, typeof(MailSubscriberDto), errorMessage);
        }
    }
}
