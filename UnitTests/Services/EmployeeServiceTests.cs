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
    public class EmployeeServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Employee>> mockRepository;
        private Mock<ISearchResult<EmployeeDto>> mockSearchResult;
        private Mock<IServiceResult<Employee>> mockServiceResult;
        private Mock<IMapper> mockMapper;
        private EmployeeService employeeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void EmployeeServiceTestsInitialize()
        {
            errorMessage = "";
            mockRepository = new Mock<IRepository<Employee>>();
            mockSearchResult = new Mock<ISearchResult<EmployeeDto>>();
            mockServiceResult = new Mock<IServiceResult<Employee>>();
            mockMapper = new Mock<IMapper>();
            employeeService = new EmployeeService(
                mockMapper.Object,
                mockRepository.Object,
                mockSearchResult.Object,
                mockServiceResult.Object);
        }

        [TestCleanup()]
        public void EmployeeServiceTestsCleanup()
        {
            employeeService = null;
        }

        private List<Employee> GetTestEmployees()
        {
            return new List<Employee>() {
                new Employee { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 },
                new Employee { Id = 2, FullName = "Jane Dane", Email = "jane@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/2", OfficeId = 2 },
                new Employee { Id = 3, FullName = "Jack Dack", Email = "jack@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/3", OfficeId = 2 }
            };
        }

        private List<EmployeeDto> GetTestEmployeeDtos()
        {
            return new List<EmployeeDto>()
            {
                new EmployeeDto { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 },
                new EmployeeDto { Id = 2, FullName = "Jane Dane", Email = "jane@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/2", OfficeId = 2 },
                new EmployeeDto { Id = 3, FullName = "Jack Dack", Email = "jack@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/3", OfficeId = 2 }
            };
        }

        private ServiceResult<Employee> GetEmployeesServiceResult()
        {
            return new ServiceResult<Employee>()
            {
                TotalCount = 3,
                Items = new List<Employee>() {
                    new Employee { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 },
                    new Employee { Id = 2, FullName = "Jane Dane", Email = "jane@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/2", OfficeId = 2 },
                    new Employee { Id = 3, FullName = "Jack Dack", Email = "jack@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/3", OfficeId = 2 }
                }
            };
        }

        #endregion

        [TestMethod]
        public async Task GetEmployeesSearchResultAsync_ReturnsSearchResultWithEmployees()
        {
            //Arrange
            ISearchResult<EmployeeDto> searchResult = null;
            int page = 1;
            int limit = 3;
            mockRepository.Setup(repo => repo.GetAsync(limit, page, null, null, null)).ReturnsAsync(GetEmployeesServiceResult());
            mockMapper.Setup(x => x.Map<IEnumerable<EmployeeDto>>(It.IsAny<IEnumerable<Employee>>())).Returns(GetTestEmployeeDtos());

            try
            {
                // Act
                searchResult = await employeeService.GetAsync(limit, page, search: "", sortField: "Id", order: OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(ISearchResult<EmployeeDto>), errorMessage);
        }

        [TestMethod]
        public async Task GetEmployeeById_ReturnsEmployeeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingEmployee = GetTestEmployees().Find(c => c.Id == id);
            mockRepository.Setup(e => e.GetAsync(id)).ReturnsAsync(existingEmployee);
            mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(GetTestEmployeeDtos().Find(e => e.Id == id));
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto());

            EmployeeDto employeeDto = null;

            try
            {
                // Act
                employeeDto = await employeeService.GetAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(employeeDto, errorMessage);
            Assert.IsInstanceOfType(employeeDto, typeof(EmployeeDto), errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task GetEmployeeById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockRepository.Setup(r => r.GetAsync(id)).Returns(value: null);
            EmployeeDto employeeDto = null;

            // Act
            try
            {
                employeeDto = await employeeService.GetAsync(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(employeeDto, errorMessage);
            mockRepository.Verify(r => r.GetAsync(id));
        }

        [TestMethod]
        public async Task CreateEmployee_ReturnsEmployeeDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of Employee domain type;
            var newEmployeeDto = new EmployeeDto() { FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 }; ;
            mockMapper.Setup(x => x.Map<Employee>(It.IsAny<EmployeeDto>())).Returns(new Employee());
            // pass the instance to repo, which should return model with created id:
            mockRepository.Setup(r => r.CreateAsync(new Employee())).ReturnsAsync(new Employee()
            {
                FullName = newEmployeeDto.FullName,
                Email = newEmployeeDto.Email,
                Position = newEmployeeDto.Position,
                Description = newEmployeeDto.Description,
                AvatarUrl = newEmployeeDto.AvatarUrl,
                OfficeId = newEmployeeDto.OfficeId
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(new EmployeeDto()
            {
                Id = int.MaxValue,
                FullName = newEmployeeDto.FullName,
                Email = newEmployeeDto.Email,
                Position = newEmployeeDto.Position,
                Description = newEmployeeDto.Description,
                AvatarUrl = newEmployeeDto.AvatarUrl,
                OfficeId = newEmployeeDto.OfficeId
            });

            EmployeeDto createdEmployeeDto = null;

            try
            {
                // Act
                createdEmployeeDto = await employeeService.CreateAsync(newEmployeeDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdEmployeeDto, errorMessage);
            Assert.IsInstanceOfType(createdEmployeeDto, typeof(EmployeeDto), errorMessage);
        }
    }
}
