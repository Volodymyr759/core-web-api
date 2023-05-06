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
    public class CompanyServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<CompanyService>> mockCompanyServiceRepository;
        private Mock<ISearchResult<CompanyServiceDto>> mockSearchResult;
        private Mock<IServiceResult<CompanyService>> mockServiceResult;
        private Mock<IMapper> mockMapper;
        private CompanyServiceBL companyServiceBL;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CompanyServiceBLTestsInitialize()
        {
            errorMessage = "";
            mockCompanyServiceRepository = new Mock<IRepository<CompanyService>>();
            mockSearchResult = new Mock<ISearchResult<CompanyServiceDto>>();
            mockServiceResult = new Mock<IServiceResult<CompanyService>>();
            mockMapper = new Mock<IMapper>();
            companyServiceBL = new CompanyServiceBL(
                mockMapper.Object,
                mockCompanyServiceRepository.Object,
                mockSearchResult.Object,
                mockServiceResult.Object);
        }

        [TestCleanup()]
        public void CompanyServiceBLTestsCleanup()
        {
            companyServiceBL = null;
        }

        private IEnumerable<CompanyService> GetTestCompanyServices()
        {
            return new List<CompanyService>() {
                new CompanyService { Id = 1, Title ="Lorem Ipsum", Description ="Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi", ImageUrl="https://somewhere.com/1", IsActive=true },
                new CompanyService { Id = 2, Title ="Sed ut perspiciatis", Description ="Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore", ImageUrl="https://somewhere.com/2", IsActive=true },
                new CompanyService { Id = 3, Title ="Magni Dolores", Description ="Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia", ImageUrl="https://somewhere.com/3", IsActive=true },
                new CompanyService { Id = 4, Title ="Nemo Enim", Description ="At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis", ImageUrl="https://somewhere.com/4", IsActive=true }
            };
        }

        private IEnumerable<CompanyServiceDto> GetTestCompanyServiceDtos()
        {
            return new List<CompanyServiceDto>() {
                new CompanyServiceDto { Id = 1, Title ="Lorem Ipsum", Description ="Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi", ImageUrl="https://somewhere.com/1", IsActive=true },
                new CompanyServiceDto { Id = 2, Title ="Sed ut perspiciatis", Description ="Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore", ImageUrl="https://somewhere.com/2", IsActive=true },
                new CompanyServiceDto { Id = 3, Title ="Magni Dolores", Description ="Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia", ImageUrl="https://somewhere.com/3", IsActive=true },
                new CompanyServiceDto { Id = 4, Title ="Nemo Enim", Description ="At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis", ImageUrl="https://somewhere.com/4", IsActive=true }
            };
        }

        private ServiceResult<CompanyService> GetCompanyServicesServiceResult()
        {
            return new ServiceResult<CompanyService>()
            {
                TotalCount = 4,
                Items = new List<CompanyService>
                {
                    new CompanyService { Id = 1, Title ="Lorem Ipsum", Description ="Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi", ImageUrl="https://somewhere.com/1", IsActive=true },
                    new CompanyService { Id = 2, Title ="Sed ut perspiciatis", Description ="Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore", ImageUrl="https://somewhere.com/2", IsActive=true },
                    new CompanyService { Id = 3, Title ="Magni Dolores", Description ="Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia", ImageUrl="https://somewhere.com/3", IsActive=true },
                    new CompanyService { Id = 4, Title ="Nemo Enim", Description ="At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis", ImageUrl="https://somewhere.com/4", IsActive=true }
                }
            };
        }

        #endregion

        [TestMethod]
        public async Task GetCompanyServicesSearchResultAsync_ReturnsSearchResultWithCompanyServices()
        {
            //Arrange
            ISearchResult<CompanyServiceDto> searchResult = null;
            int page = 1;
            int limit = 3;
            CompanyServiceStatus companyServiceStatus = CompanyServiceStatus.All;
            mockCompanyServiceRepository.Setup(repo => repo.GetAsync(limit, page, null, null, null)).ReturnsAsync(GetCompanyServicesServiceResult());
            mockMapper.Setup(x => x.Map<IEnumerable<CompanyServiceDto>>(It.IsAny<IEnumerable<CompanyService>>())).Returns(GetTestCompanyServiceDtos());

            try
            {
                // Act
                searchResult = await companyServiceBL.GetAsync(limit, page, companyServiceStatus, OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(ISearchResult<CompanyServiceDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetCompanyServiceById_ReturnsCompanyServiceDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingCompanyService = ((List<CompanyService>)GetTestCompanyServices()).Find(c => c.Id == id);
            mockCompanyServiceRepository.Setup(r => r.GetAsync(id)).ReturnsAsync(existingCompanyService);
            mockMapper.Setup(x => x.Map<CompanyServiceDto>(It.IsAny<CompanyService>()))
                .Returns(((List<CompanyServiceDto>)GetTestCompanyServiceDtos()).Find(c => c.Id == id));
            CompanyServiceDto companyServiceDto = null;

            try
            {
                // Act
                companyServiceDto = await companyServiceBL.GetAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(companyServiceDto, errorMessage);
            Assert.IsInstanceOfType(companyServiceDto, typeof(CompanyServiceDto), errorMessage);
            mockCompanyServiceRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetCompanyServiceById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCompanyServiceRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            CompanyServiceDto companyServiceDto = null;

            try
            {
                // Act
                companyServiceDto = await companyServiceBL.GetAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(companyServiceDto, errorMessage);
            mockCompanyServiceRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateCompanyService_ReturnsCompanyServiceDto()
        {
            // Arrange 
            // scenario:
            // service recievs CompanyServiceDto model and should map it to instance of domain type CompanyService;
            var newCompanyServiceDto = new CompanyServiceDto() { Title = "Testing service", Description = "Test description", ImageUrl = "https://somewhere/111", IsActive = true };
            mockMapper.Setup(x => x.Map<CompanyService>(It.IsAny<CompanyServiceDto>())).Returns(new CompanyService());
            // pass the instance of CompanyService to repo, which should return model with created id:
            mockCompanyServiceRepository.Setup(r => r.CreateAsync(new CompanyService())).ReturnsAsync(new CompanyService()
            {
                Id = int.MaxValue,
                Title = newCompanyServiceDto.Title,
                Description = newCompanyServiceDto.Description,
                ImageUrl = newCompanyServiceDto.ImageUrl,
                IsActive = newCompanyServiceDto.IsActive
            });
            // service maps CompanyService-object from db back to CompanyServiceDto type:
            mockMapper.Setup(x => x.Map<CompanyServiceDto>(It.IsAny<CompanyService>())).Returns(new CompanyServiceDto()
            {
                Id = int.MaxValue,
                Title = newCompanyServiceDto.Title,
                Description = newCompanyServiceDto.Description,
                ImageUrl = newCompanyServiceDto.ImageUrl,
                IsActive = newCompanyServiceDto.IsActive
            });

            CompanyServiceDto createdCompanyServiceDto = null;

            try
            {
                // Act
                createdCompanyServiceDto = await companyServiceBL.CreateAsync(newCompanyServiceDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCompanyServiceDto, errorMessage);
            Assert.IsInstanceOfType(createdCompanyServiceDto, typeof(CompanyServiceDto), errorMessage);
        }
    }
}
