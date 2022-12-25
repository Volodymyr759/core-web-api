using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.TenantService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace UnitTests.Services
{
    [TestClass]
    public class TenantServiceTests
    {
        #region Private Members

        private string errorMessage;

        private Mock<IRepository<Tenant>> mockTenantRepository;

        private Mock<IMapper> mockMapper;

        private TenantService tenantServise;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void TenantServiceTestsInitialize()
        {
            errorMessage = "";
            mockTenantRepository = new Mock<IRepository<Tenant>>();
            mockMapper = new Mock<IMapper>();
            tenantServise = new TenantService(mockMapper.Object, mockTenantRepository.Object);
        }

        [TestCleanup()]
        public void TenantServiceTestsCleanup()
        {
            tenantServise = null;
        }

        #endregion

        [TestMethod]
        public void GetTenantById_ReturnsTenantDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingTenantDto = new TenantDto { Id = 1, FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            mockTenantRepository.Setup(r => r.Get(t => t.Id == id)).Returns(GetTestTenantById(id));
            mockMapper.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>())).Returns(existingTenantDto);
            TenantDto tenantDto = null;

            try
            {
                // Act
                tenantDto = tenantServise.GetTenantById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(tenantDto, errorMessage);
            Assert.IsInstanceOfType(tenantDto, typeof(TenantDto), errorMessage);
        }

        [TestMethod]
        public void GetTenantById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            TenantDto tenantDto = null;
            mockTenantRepository.Setup(r => r.Get(t => t.Id == id)).Returns(new Tenant());
            mockMapper.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>())).Returns(tenantDto);

            try
            {
                // Act
                tenantDto = tenantServise.GetTenantById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(tenantDto, errorMessage);
        }

        [TestMethod]
        public void CreateTenant_ReturnsTenantDto()
        {
            //Arrange
            var createTenantDto = new CreateTenantDto { FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            var tenant = new Tenant { Id = 1, FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            mockMapper.Setup(x => x.Map<Tenant>(It.IsAny<CreateTenantDto>())).Returns(new Tenant());
            mockTenantRepository.Setup(r => r.Create(tenant)).Returns(GetTestTenantById(1));
            mockMapper.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>())).Returns(new TenantDto());
            TenantDto tenantDto = null;

            try
            {
                // Act
                tenantDto = tenantServise.CreateTenant(createTenantDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(tenantDto, errorMessage);
            Assert.IsInstanceOfType(tenantDto, typeof(TenantDto), errorMessage);
        }

        [TestMethod]
        public void UpdateTenant_ReturnsUpdatedTenantDto()
        {
            //Arrange
            var tenantDtoToUpdate = new TenantDto { Id = 1, FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            var tenant = new Tenant { Id = 1, FirstName = "First Name", LastName = "Last Name", Email = "q@q.com", Phone = "+123123123" };
            mockMapper.Setup(x => x.Map<Tenant>(It.IsAny<TenantDto>())).Returns(GetTestTenantById(1));
            mockTenantRepository.Setup(r => r.Update(tenant)).Returns(GetTestTenantById(1));
            mockMapper.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>())).Returns(new TenantDto());

            TenantDto tenantDto = null;

            try
            {
                // Act
                tenantDto = tenantServise.UpdateTenant(tenantDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(tenantDto, errorMessage);
            Assert.IsInstanceOfType(tenantDto, typeof(TenantDto), errorMessage);
        }

        private Tenant GetTestTenantById(int id)
        {
            var tenant = (id != 0) ? new Tenant { Id = id, FirstName = "First Name", LastName = "Last Name", Email = "email@gmail.com", Phone = "+123123123" } : null;
            return tenant;
        }
    }
}
