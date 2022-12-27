using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.CompanyServiceBL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    public class CompanyServiceTests
    {
        #region Private Members

        private string errorMessage;

        private Mock<IRepository<CompanyService>> mockCompanyServiceRepository;

        private Mock<IMapper> mockMapper;

        private CompanyServiceBL companyServiceBL;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CompanyServiceBLTestsInitialize()
        {
            errorMessage = "";
            mockCompanyServiceRepository = new Mock<IRepository<CompanyService>>();
            mockMapper = new Mock<IMapper>();
            companyServiceBL = new CompanyServiceBL(mockMapper.Object, mockCompanyServiceRepository.Object);
        }

        [TestCleanup()]
        public void CompanyServiceBLTestsCleanup()
        {
            companyServiceBL = null;
        }

        #endregion

        [TestMethod]
        public void GetAllCompanyServices_ReturnsListOfCompanyServices()
        {
            //Arrange
            IEnumerable<CompanyServiceDto> companyServiceDtos = null;
            int page = 1;
            int limit = 3;
            mockCompanyServiceRepository.Setup(repo => repo.GetAll()).Returns(GetTestCompanyServices());
            mockMapper.Setup(x => x.Map<IEnumerable<CompanyServiceDto>>(It.IsAny<IEnumerable<CompanyService>>())).Returns(GetTestCompanyServiceDtos());

            try
            {
                // Act
                companyServiceDtos = companyServiceBL.GetAllCompanyServices(page, "asc", limit);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(companyServiceDtos, errorMessage);
            Assert.IsTrue(((List<CompanyServiceDto>)companyServiceDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(companyServiceDtos, typeof(IEnumerable<CompanyServiceDto>), errorMessage);
        }

        [TestMethod]
        public void GetCompanyServiceById_ReturnsCompanyServiceDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingCompanyService = ((List<CompanyService>)GetTestCompanyServices()).Find(c => c.Id == id);
            mockCompanyServiceRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingCompanyService);
            mockMapper.Setup(x => x.Map<CompanyServiceDto>(It.IsAny<CompanyService>()))
                .Returns(((List<CompanyServiceDto>)GetTestCompanyServiceDtos()).Find(c => c.Id == id));
            CompanyServiceDto companyServiceDto = null;

            try
            {
                // Act
                companyServiceDto = companyServiceBL.GetCompanyServiceById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(companyServiceDto, errorMessage);
            Assert.IsInstanceOfType(companyServiceDto, typeof(CompanyServiceDto), errorMessage);
        }

        [TestMethod]
        public void GetCompanyServiceById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCompanyServiceRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            CompanyServiceDto companyServiceDto = null;

            try
            {
                // Act
                companyServiceDto = companyServiceBL.GetCompanyServiceById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(companyServiceDto, errorMessage);
        }

        [TestMethod]
        public void CreateCompanyService_ReturnsCompanyServiceDto()
        {
            // Arrange 
            // scenario:
            // service recievs CompanyServiceDto model and should map it to instance of domain type CompanyService;
            var newCompanyServiceDto = new CompanyServiceDto() { Title = "Testing service", Description = "Test description", ImageUrl = "https://somewhere/111", IsActive = true };
            mockMapper.Setup(x => x.Map<CompanyService>(It.IsAny<CompanyServiceDto>())).Returns(new CompanyService());
            // pass the instance of CompanyService to repo, which should return model with created id:
            mockCompanyServiceRepository.Setup(r => r.Create(new CompanyService())).Returns(new CompanyService()
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
                createdCompanyServiceDto = companyServiceBL.CreateCompanyService(newCompanyServiceDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCompanyServiceDto, errorMessage);
            Assert.IsInstanceOfType(createdCompanyServiceDto, typeof(CompanyServiceDto), errorMessage);
        }

        [TestMethod]
        public void UpdateCompanyService_ReturnsUpdatedCompanyServiceDto()
        {
            //Arrange
            // the same scenario like in 'Create' method
            var companyServiceDtoToUpdate = new CompanyServiceDto() { Title = "Testing service", Description = "Test description", ImageUrl = "https://somewhere/111", IsActive = true };
            mockMapper.Setup(x => x.Map<CompanyService>(It.IsAny<CompanyServiceDto>())).Returns(new CompanyService());
            mockCompanyServiceRepository.Setup(r => r.Update(new CompanyService())).Returns(new CompanyService()
            {
                Id = int.MaxValue,
                Title = companyServiceDtoToUpdate.Title,
                Description = companyServiceDtoToUpdate.Description,
                ImageUrl = companyServiceDtoToUpdate.ImageUrl,
                IsActive = companyServiceDtoToUpdate.IsActive
            });
            mockMapper.Setup(x => x.Map<CompanyServiceDto>(It.IsAny<CompanyService>())).Returns(new CompanyServiceDto()
            {
                Id = int.MaxValue,
                Title = companyServiceDtoToUpdate.Title,
                Description = companyServiceDtoToUpdate.Description,
                ImageUrl = companyServiceDtoToUpdate.ImageUrl,
                IsActive = companyServiceDtoToUpdate.IsActive
            });

            CompanyServiceDto createdCompanyServiceDto = null;

            try
            {
                // Act
                createdCompanyServiceDto = companyServiceBL.UpdateCompanyService(companyServiceDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCompanyServiceDto, errorMessage);
            Assert.IsInstanceOfType(createdCompanyServiceDto, typeof(CompanyServiceDto), errorMessage);
        }

        [TestMethod]
        public void DeleteCompanyServiceById_ReturnsCompanyServiceDto()
        {
            // Arrange
            // scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockCompanyServiceRepository.Setup(r => r.Delete(id)).Returns(((List<CompanyService>)GetTestCompanyServices()).Find(c => c.Id == id));
            // since repo.delete(int id) returns origin CompanyService-object - possible to map it to dto object and give it back:
            mockMapper.Setup(x => x.Map<CompanyServiceDto>(It.IsAny<CompanyService>())).Returns(((List<CompanyServiceDto>)GetTestCompanyServiceDtos()).Find(c => c.Id == id));

            CompanyServiceDto companyServiceDto = null;

            try
            {
                // Act
                companyServiceDto = companyServiceBL.DeleteCompanyService(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(companyServiceDto, errorMessage);
            Assert.IsInstanceOfType(companyServiceDto, typeof(CompanyServiceDto), errorMessage);
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

    }
}
