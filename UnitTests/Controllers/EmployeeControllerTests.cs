using CoreWebApi.Controllers.Employee;
using CoreWebApi.Services.EmployeeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Controllers
{
    [TestClass]
    public class EmployeeControllerTests
    {
        #region Private members

        private string errorMessage;
        private EmployeeController employeeController;
        private Mock<IEmployeeService> mockEmployeeService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void EmployeeControllerTestInitialize()
        {
            errorMessage = "";
            mockEmployeeService = new Mock<IEmployeeService>();
            employeeController = new EmployeeController(mockEmployeeService.Object);
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
        public void GetById_ReturnsOkWithEmployeeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            mockEmployeeService.Setup(r => r.GetEmployeeById(id)).Returns(GetTestEmployeeDtoById(id));
            OkObjectResult result = null;

            try
            {
                // Act
                result = employeeController.GetById(id) as OkObjectResult;
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
            mockEmployeeService.Verify(r => r.GetEmployeeById(id));
        }

        [TestMethod]
        public void GetById_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockEmployeeService.Setup(r => r.GetEmployeeById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = employeeController.GetById(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockEmployeeService.Verify(r => r.GetEmployeeById(id));
        }

        [TestMethod]
        public void Create_ReturnsCreatedEmployeeDtoByValidArg()
        {
            //Arrange
            var createEmployeeDto = GetTestEmployeeDtoById(1);
            mockEmployeeService.Setup(r => r.CreateEmployee(createEmployeeDto)).Returns(GetTestEmployeeDtoById(1));
            CreatedResult result = null;

            try
            {
                // Act
                result = employeeController.Create(createEmployeeDto) as CreatedResult;
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
            mockEmployeeService.Verify(r => r.CreateEmployee(createEmployeeDto));
        }

        [TestMethod]
        public void Create_ReturnsBadRequestByInvalidArg()
        {
            //Arrange
            int id = 1;
            var createEmployeeDto = GetTestEmployeeDtoById(id); // too long Name string
            employeeController.ModelState.AddModelError("Name", "Employee name (1-20 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Create(createEmployeeDto) as BadRequestObjectResult;
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
        public void Update_ReturnsEmployeeDtoByValidArg()
        {
            //Arrange
            var employeeDtoToUpdate = GetTestEmployeeDtoById(1);
            mockEmployeeService.Setup(r => r.GetEmployeeById(employeeDtoToUpdate.Id)).Returns(employeeDtoToUpdate);
            mockEmployeeService.Setup(r => r.UpdateEmployee(employeeDtoToUpdate)).Returns(employeeDtoToUpdate);
            OkObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Update(employeeDtoToUpdate) as OkObjectResult;
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
            mockEmployeeService.Verify(r => r.UpdateEmployee(employeeDtoToUpdate));
        }

        [TestMethod]
        public void Update_ReturnsNotFoundByWrongIdInArg()
        {
            //Arrange
            var employeeDtoToUpdate = GetTestEmployeeDtoById(1);
            employeeDtoToUpdate.Id = 0; // wrong id
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Update(employeeDtoToUpdate) as NotFoundObjectResult;
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
        public void Update_ReturnsBadRequestByWrongArg()
        {
            //Arrange
            int id = 1;
            var employeeDtoToUpdate = GetTestEmployeeDtoById(id);
            employeeController.ModelState.AddModelError("Name", "Employee name (1-20 characters) is required.");
            BadRequestObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Update(employeeDtoToUpdate) as BadRequestObjectResult;
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
        public void Delete_ReturnsOkWithEmployeeDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var employeeToDelete = GetTestEmployeeDtoById(id);
            mockEmployeeService.Setup(r => r.GetEmployeeById(id)).Returns(employeeToDelete);
            OkObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Delete(id) as OkObjectResult;
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
            mockEmployeeService.Verify(r => r.GetEmployeeById(id));
        }

        [TestMethod]
        public void Delete_ReturnsNotFoundByWrongId()
        {
            //Arrange
            int id = 0;// wrong id
            mockEmployeeService.Setup(r => r.GetEmployeeById(id)).Returns(value: null);
            NotFoundObjectResult result = null;

            try
            {
                // Act
                result = employeeController.Delete(id) as NotFoundObjectResult;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(result, errorMessage);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult), errorMessage);
            mockEmployeeService.Verify(r => r.GetEmployeeById(id));
        }

        #endregion
    }
}
