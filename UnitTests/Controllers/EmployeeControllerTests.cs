using CoreWebApi.Controllers;
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
    public class EmployeeControllerTests
    {
        #region Private members

        private string errorMessage;
        private EmployeeController employeeController;
        private Mock<IEmployeeService> mockEmployeeService;
        private Mock<IOfficeService> mockOfficeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void EmployeeControllerTestInitialize()
        {
            errorMessage = "";
            mockEmployeeService = new Mock<IEmployeeService>();
            mockOfficeService = new Mock<IOfficeService>();
            employeeController = new EmployeeController(mockEmployeeService.Object, mockOfficeService.Object);
        }

        [TestCleanup()]
        public void EmployeeControllerTestsCleanup()
        {
            employeeController = null;
        }

        private EmployeeDto GetTestEmployeeDtoById(int id)
        {
            return new EmployeeDto { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 };
        }

        private IEnumerable<EmployeeDto> GetTestEmployeeDtos()
        {
            return new List<EmployeeDto>() {
                new EmployeeDto { Id = 1, FullName = "John Done", Email = "john@gmail.com", Position = "CEO", Description = "CEO description", AvatarUrl = "https://www.somewhere.com/1", OfficeId = 1 },
                new EmployeeDto { Id = 2, FullName = "Jane Dane", Email = "jane@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/2", OfficeId = 2 },
                new EmployeeDto { Id = 3, FullName = "Jack Dack", Email = "jack@gmail.com", Position = "Developer", Description = "Developer description", AvatarUrl = "https://www.somewhere.com/3", OfficeId = 2 }
            };
        }

        #endregion

        #region Tests

        [TestMethod]
        public async Task GetById_ReturnsOkWithEmployeeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockEmployeeService.Setup(r => r.GetEmployeeByIdAsync(id)).ReturnsAsync(GetTestEmployeeDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.GetByIdAsync(id) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(EmployeeDto), errorMessage);
            mockEmployeeService.Verify(r => r.GetEmployeeByIdAsync(id));
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockEmployeeService.Setup(r => r.GetEmployeeByIdAsync(id)).ReturnsAsync(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.GetByIdAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockEmployeeService.Verify(r => r.GetEmployeeByIdAsync(id));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedEmployeeDtoByValidArg()
        {
            //Arrange
            var createEmployeeDto = GetTestEmployeeDtoById(1);
            mockEmployeeService.Setup(r => r.CreateEmployeeAsync(createEmployeeDto)).ReturnsAsync(GetTestEmployeeDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = await employeeController.CreateAsync(createEmployeeDto) as CreatedResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(CreatedResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(EmployeeDto), errorMessage);
            mockEmployeeService.Verify(r => r.CreateEmployeeAsync(createEmployeeDto));
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createEmployeeDto = GetTestEmployeeDtoById(id); // too long Name string
            employeeController.ModelState.AddModelError("Name", "Employee name (1-20 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.CreateAsync(createEmployeeDto) as BadRequestObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), errorMessage);
        }

        [TestMethod]
        public async Task Update_ReturnsEmployeeDtoByValidArg()
        {
            //Arrange
            int id = 1;
            var employeeDtoToUpdate = GetTestEmployeeDtoById(id);
            mockEmployeeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockEmployeeService.Setup(r => r.UpdateEmployeeAsync(employeeDtoToUpdate)).Returns(Task.CompletedTask);
            OkObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.UpdateAsync(employeeDtoToUpdate) as OkObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult), errorMessage);
            Assert.IsNotNull(result.Value, errorMessage);
            Assert.IsInstanceOfType(result.Value, typeof(EmployeeDto), errorMessage);
            mockEmployeeService.Verify(r => r.UpdateEmployeeAsync(employeeDtoToUpdate));
            mockEmployeeService.Verify(r => r.IsExistAsync(id));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var employeeDtoToUpdate = GetTestEmployeeDtoById(1);
            employeeDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.UpdateAsync(employeeDtoToUpdate) as NotFoundObjectResult;
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
        public async Task Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int id = 1;
            var employeeDtoToUpdate = GetTestEmployeeDtoById(id);
            employeeController.ModelState.AddModelError("Name", "Employee name (1-20 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.UpdateAsync(employeeDtoToUpdate) as BadRequestObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult), errorMessage);
        }

        [TestMethod]
        public async Task Delete_ReturnsOkByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockEmployeeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(true));
            mockEmployeeService.Setup(r => r.DeleteEmployeeAsync(id)).Returns(Task.CompletedTask);
            OkResult result = null;

            try
            {
                // Act
                result = await employeeController.DeleteAsync(id) as OkResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(OkResult), errorMessage);
            mockEmployeeService.Verify(r => r.IsExistAsync(id));
            mockEmployeeService.Verify(r => r.DeleteEmployeeAsync(id));
        }

        [TestMethod]
        public async Task Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockEmployeeService.Setup(r => r.IsExistAsync(id)).Returns(Task.FromResult(false));
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = await employeeController.DeleteAsync(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockEmployeeService.Verify(r => r.IsExistAsync(id));
        }

        #endregion
    }
}
