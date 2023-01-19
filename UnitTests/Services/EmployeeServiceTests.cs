using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using CoreWebApi.Services.EmployeeService;
using CoreWebApi.Services.OfficeService;
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
        private Mock<IRepository<Employee>> mockEmployeeRepository;
        private Mock<IRepository<Office>> mockOfficeRepository;
        private Mock<IMapper> mockMapper;
        private EmployeeService employeeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void EmployeeServiceTestsInitialize()
        {
            errorMessage = "";
            mockEmployeeRepository = new Mock<IRepository<Employee>>();
            mockOfficeRepository = new Mock<IRepository<Office>>();
            mockMapper = new Mock<IMapper>();
            employeeService = new EmployeeService(
                mockMapper.Object,
                mockEmployeeRepository.Object,
                mockOfficeRepository.Object);
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

        #endregion

        [TestMethod]
        public async Task GetEmployeesSearchResultAsync_ReturnsSearchResultWithEmployees()
        {
            //Arrange
            SearchResult<EmployeeDto> searchResult = null;
            int page = 1;
            int limit = 3;
            mockEmployeeRepository.Setup(repo => repo.GetAllAsync(null, null)).ReturnsAsync(GetTestEmployees());
            mockMapper.Setup(x => x.Map<IEnumerable<EmployeeDto>>(It.IsAny<IEnumerable<Employee>>())).Returns(GetTestEmployeeDtos());

            try
            {
                // Act
                searchResult = await employeeService.GetEmployeesSearchResultAsync(limit, page, search: "", sort_field: "Id", order: OrderType.Ascending);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(searchResult, errorMessage);
            Assert.IsTrue(searchResult.ItemList.Count == limit, errorMessage);
            Assert.IsInstanceOfType(searchResult, typeof(SearchResult<EmployeeDto>), errorMessage);
        }

        [TestMethod]
        public void GetEmployeeById_ReturnsEmployeeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingEmployee = GetTestEmployees().Find(c => c.Id == id);
            mockEmployeeRepository.Setup(e => e.Get(e => e.Id == id)).Returns(existingEmployee);
            mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(GetTestEmployeeDtos().Find(e => e.Id == id));
            mockOfficeRepository.Setup(r => r.Get(t => t.Id == id)).Returns(new Office());
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto());

            EmployeeDto employeeDto = null;

            try
            {
                // Act
                employeeDto = employeeService.GetEmployeeById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(employeeDto, errorMessage);
            Assert.IsInstanceOfType(employeeDto, typeof(EmployeeDto), errorMessage);
            Assert.IsNotNull(employeeDto.OfficeDto, errorMessage);
        }

        [TestMethod]
        public void GetEmployeeById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockEmployeeRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            EmployeeDto employeeDto = null;

            // Act
            try
            {
                employeeDto = employeeService.GetEmployeeById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(employeeDto, errorMessage);
        }

        [TestMethod]
        public void CreateEmployee_ReturnsEmployeeDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of Employee domain type;
            var newEmployeeDto = new EmployeeDto() { FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 }; ;
            mockMapper.Setup(x => x.Map<Employee>(It.IsAny<EmployeeDto>())).Returns(new Employee());
            // pass the instance to repo, which should return model with created id:
            mockEmployeeRepository.Setup(r => r.Create(new Employee())).Returns(new Employee()
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
                createdEmployeeDto = employeeService.CreateEmployee(newEmployeeDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdEmployeeDto, errorMessage);
            Assert.IsInstanceOfType(createdEmployeeDto, typeof(EmployeeDto), errorMessage);
        }

        [TestMethod]
        public void UpdateEmployee_ReturnsUpdatedEmployeeDto()
        {
            //Arrange the same scenario like in 'Create' method
            var employeeDtoToUpdate = new EmployeeDto() { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 };
            mockMapper.Setup(x => x.Map<Employee>(It.IsAny<EmployeeDto>())).Returns(new Employee());
            mockEmployeeRepository.Setup(r => r.Update(new Employee())).Returns(new Employee()
            {
                Id = int.MaxValue,
                FullName = employeeDtoToUpdate.FullName,
                Email = employeeDtoToUpdate.Email,
                Position = employeeDtoToUpdate.Position,
                Description = employeeDtoToUpdate.Description,
                AvatarUrl = employeeDtoToUpdate.AvatarUrl,
                OfficeId = employeeDtoToUpdate.OfficeId
            });
            mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(new EmployeeDto()
            {
                Id = int.MaxValue,
                FullName = employeeDtoToUpdate.FullName,
                Email = employeeDtoToUpdate.Email,
                Position = employeeDtoToUpdate.Position,
                Description = employeeDtoToUpdate.Description,
                AvatarUrl = employeeDtoToUpdate.AvatarUrl,
                OfficeId = employeeDtoToUpdate.OfficeId
            });

            EmployeeDto updatedEmployeeDto = null;

            try
            {
                // Act
                updatedEmployeeDto = employeeService.UpdateEmployee(employeeDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(updatedEmployeeDto, errorMessage);
            Assert.IsInstanceOfType(updatedEmployeeDto, typeof(EmployeeDto), errorMessage);
        }

        [TestMethod]
        public void DeleteEmployeeById_ReturnsEmployeeDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 3;
            mockEmployeeRepository.Setup(r => r.Delete(id)).Returns(GetTestEmployees().Find(c => c.Id == id));
            // since repo.delete(int id) returns origin Employee-object - possible to map it to dto and give it back:
            mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(GetTestEmployeeDtos().Find(c => c.Id == id));

            EmployeeDto employeeDto = null;

            // Act
            try
            {
                employeeDto = employeeService.DeleteEmployee(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(employeeDto, errorMessage);
            Assert.IsInstanceOfType(employeeDto, typeof(EmployeeDto), errorMessage);
        }
    }
}
