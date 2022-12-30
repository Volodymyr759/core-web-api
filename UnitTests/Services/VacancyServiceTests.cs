using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.CandidateService;
using CoreWebApi.Services.OfficeService;
using CoreWebApi.Services.VacancyService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class VacancyServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Vacancy>> mockVacancyRepository;
        private Mock<IRepository<Office>> mockOfficeRepository;
        private Mock<IRepository<Candidate>> mockCandidateRepository;
        private Mock<IMapper> mockMapper;
        private VacancyService vacancyService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void VacancyServiceTestsInitialize()
        {
            errorMessage = "";
            mockVacancyRepository = new Mock<IRepository<Vacancy>>();
            mockOfficeRepository = new Mock<IRepository<Office>>();
            mockCandidateRepository = new Mock<IRepository<Candidate>>();
            mockMapper = new Mock<IMapper>();
            vacancyService = new VacancyService(
                mockMapper.Object,
                mockVacancyRepository.Object,
                mockOfficeRepository.Object,
                mockCandidateRepository.Object);
        }

        [TestCleanup()]
        public void VacancyServiceTestsCleanup()
        {
            vacancyService = null;
        }

        private List<Vacancy> GetTestVacancies()
        {
            return new List<Vacancy>() {
                new Vacancy { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 },
                new Vacancy { Id = 2, Title = "Junior JavaScrip Frontend Developer", Description = "Test description 2", Previews = 0, IsActive = true, OfficeId = 1 },
                new Vacancy { Id = 3, Title = "Senior JavaScrip Frontend Developer", Description = "Test description 3", Previews = 0, IsActive = true, OfficeId = 1 }
            };
        }

        private List<VacancyDto> GetTestVacancyDtos()
        {
            return new List<VacancyDto>()
            {
                new VacancyDto { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 2, Title = "Junior JavaScrip Frontend Developer", Description = "Test description 2", Previews = 0, IsActive = true, OfficeId = 1 },
                new VacancyDto { Id = 3, Title = "Senior JavaScrip Frontend Developer", Description = "Test description 3", Previews = 0, IsActive = true, OfficeId = 1 }
            };
        }

        #endregion

        [TestMethod]
        public void GetAllVacancies_ReturnsListOfVacancies()
        {
            //Arrange
            IEnumerable<VacancyDto> vacancyDtos = null;
            int page = 1;
            int limit = 3;
            mockVacancyRepository.Setup(repo => repo.GetAll()).Returns(GetTestVacancies());
            mockMapper.Setup(x => x.Map<IEnumerable<VacancyDto>>(It.IsAny<IEnumerable<Vacancy>>())).Returns(GetTestVacancyDtos());

            try
            {
                // Act
                vacancyDtos = vacancyService.GetAllVacancies(10, page, "", "Title", "asc");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(vacancyDtos, errorMessage);
            Assert.IsTrue(((List<VacancyDto>)vacancyDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(vacancyDtos, typeof(IEnumerable<VacancyDto>), errorMessage);
        }

        [TestMethod]
        public void GetVacancyById_ReturnsVacancyDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingVacancy = GetTestVacancies().Find(c => c.Id == id);
            mockVacancyRepository.Setup(r => r.Get(t => t.Id == id)).Returns(existingVacancy);
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(GetTestVacancyDtos().Find(c => c.Id == id));

            mockOfficeRepository.Setup(e => e.Get(existingVacancy.OfficeId)).Returns(new Office());
            mockMapper.Setup(x => x.Map<OfficeDto>(It.IsAny<Office>())).Returns(new OfficeDto());

            mockCandidateRepository.Setup(v => v.GetAll()).Returns(new List<Candidate>());
            mockMapper.Setup(x => x.Map<List<CandidateDto>>(It.IsAny<List<Candidate>>())).Returns(new List<CandidateDto>());

            VacancyDto vacancyDto = null;

            try
            {
                // Act
                vacancyDto = vacancyService.GetVacancyById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(vacancyDto, errorMessage);
            Assert.IsInstanceOfType(vacancyDto, typeof(VacancyDto), errorMessage);
            Assert.IsNotNull(vacancyDto.OfficeDto, errorMessage);
            Assert.IsNotNull(vacancyDto.CandidateDtos, errorMessage);
        }

        [TestMethod]
        public void GetVacancyById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockVacancyRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            VacancyDto vacancyDto = null;

            try
            {
                // Act
                vacancyDto = vacancyService.GetVacancyById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(vacancyDto, errorMessage);
        }

        [TestMethod]
        public void CreateVacancy_ReturnsVacancyDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newVacancyDto = new VacancyDto() { Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 };
            mockMapper.Setup(x => x.Map<Vacancy>(It.IsAny<VacancyDto>())).Returns(new Vacancy());
            // pass the instance to repo, which should return model with created id:
            mockVacancyRepository.Setup(r => r.Create(new Vacancy())).Returns(new Vacancy()
            {
                Id = int.MaxValue,
                Title = newVacancyDto.Title,
                Description = newVacancyDto.Description,
                Previews = newVacancyDto.Previews,
                IsActive = newVacancyDto.IsActive,
                OfficeId = newVacancyDto.OfficeId
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(new VacancyDto()
            {
                Id = int.MaxValue,
                Title = newVacancyDto.Title,
                Description = newVacancyDto.Description,
                Previews = newVacancyDto.Previews,
                IsActive = newVacancyDto.IsActive,
                OfficeId = newVacancyDto.OfficeId
            });

            VacancyDto createdVacancyDto = null;

            try
            {
                // Act
                createdVacancyDto = vacancyService.CreateVacancy(newVacancyDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdVacancyDto, errorMessage);
            Assert.IsInstanceOfType(createdVacancyDto, typeof(VacancyDto), errorMessage);
        }

        [TestMethod]
        public void UpdateVacancy_ReturnsUpdatedVacancyDto()
        {
            //Arrange the same scenario like in 'Create' method
            var vacancyDtoToUpdate = new VacancyDto() { Id = 1, Title = ".Net Developer", Description = "Test description 1", Previews = 1, IsActive = true, OfficeId = 1 };
            mockMapper.Setup(x => x.Map<Vacancy>(It.IsAny<VacancyDto>())).Returns(new Vacancy());
            mockVacancyRepository.Setup(r => r.Update(new Vacancy())).Returns(new Vacancy()
            {
                Id = int.MaxValue,
                Title = vacancyDtoToUpdate.Title,
                Description = vacancyDtoToUpdate.Description,
                Previews = vacancyDtoToUpdate.Previews,
                IsActive = vacancyDtoToUpdate.IsActive,
                OfficeId = vacancyDtoToUpdate.OfficeId
            });
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(new VacancyDto()
            {
                Id = int.MaxValue,
                Title = vacancyDtoToUpdate.Title,
                Description = vacancyDtoToUpdate.Description,
                Previews = vacancyDtoToUpdate.Previews,
                IsActive = vacancyDtoToUpdate.IsActive,
                OfficeId = vacancyDtoToUpdate.OfficeId
            });

            VacancyDto updatedVacancyDto = null;

            try
            {
                // Act
                updatedVacancyDto = vacancyService.UpdateVacancy(vacancyDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(updatedVacancyDto, errorMessage);
            Assert.IsInstanceOfType(updatedVacancyDto, typeof(VacancyDto), errorMessage);
        }

        [TestMethod]
        public void DeleteVacancyById_ReturnsVacancyDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 1;
            mockVacancyRepository.Setup(r => r.Delete(id)).Returns(GetTestVacancies().Find(c => c.Id == id));
            // since repo.delete(int id) returns origin Vacancy-object - possible to map it to dto and give it back:
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(GetTestVacancyDtos().Find(c => c.Id == id));

            VacancyDto vacancyDto = null;

            try
            {
                // Act
                vacancyDto = vacancyService.DeleteVacancy(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(vacancyDto, errorMessage);
            Assert.IsInstanceOfType(vacancyDto, typeof(VacancyDto), errorMessage);
        }

    }
}
