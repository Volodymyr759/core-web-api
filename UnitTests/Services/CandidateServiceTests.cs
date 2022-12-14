using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.CandidateService;
using CoreWebApi.Services.VacancyService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTests.Services
{
    [TestClass]
    public class CandidateServiceTests
    {
        #region Private Members

        private string errorMessage;
        private Mock<IRepository<Candidate>> mockCandidateRepository;
        private Mock<IRepository<Vacancy>> mockVacancyRepository;
        private Mock<IMapper> mockMapper;
        private CandidateService candidateService;

        #endregion

        #region Utilities

        [TestInitialize()]
        public void CandidateServiceTestsInitialize()
        {
            errorMessage = "";
            mockCandidateRepository = new Mock<IRepository<Candidate>>();
            mockVacancyRepository = new Mock<IRepository<Vacancy>>();
            mockMapper = new Mock<IMapper>();
            candidateService = new CandidateService(
                mockMapper.Object,
                mockCandidateRepository.Object,
                mockVacancyRepository.Object);
        }

        [TestCleanup()]
        public void CandidateServiceTestsCleanup()
        {
            candidateService = null;
        }

        private List<Candidate> GetTestCandidates()
        {
            return new List<Candidate>() {
                new Candidate { Id = 1, FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "Test note 1", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new Candidate { Id = 2, FullName = "Merelin Monroe", Email = "merelin@gmail.com", Phone = "+1234567892", Notes = "Test note 2", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new Candidate { Id = 3, FullName = "Julia Roberts", Email = "julia@gmail.com", Phone = "+1234567893", Notes = "Test note 3", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 }
            };
        }

        private List<CandidateDto> GetTestCandidateDtos()
        {
            return new List<CandidateDto>()
            {
                new CandidateDto { Id = 1, FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new CandidateDto { Id = 2, FullName = "Merelin Monroe", Email = "merelin@gmail.com", Phone = "+1234567892", Notes = "", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 },
                new CandidateDto { Id = 3, FullName = "Julia Roberts", Email = "julia@gmail.com", Phone = "+1234567893", Notes = "", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 }
            };
        }

        #endregion

        [TestMethod]
        public void GetAllCandidates_ReturnsListOfCandidates()
        {
            //Arrange
            IEnumerable<CandidateDto> candidateDtos = null;
            int page = 1;
            int limit = 3;
            mockCandidateRepository.Setup(repo => repo.GetAll()).Returns(GetTestCandidates());
            mockMapper.Setup(x => x.Map<IEnumerable<CandidateDto>>(It.IsAny<IEnumerable<Candidate>>())).Returns(GetTestCandidateDtos());

            try
            {
                // Act
                candidateDtos = candidateService.GetAllCandidates(10, page, "", "Title", "asc");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(candidateDtos, errorMessage);
            Assert.IsTrue(((List<CandidateDto>)candidateDtos).Count == limit, errorMessage);
            Assert.IsInstanceOfType(candidateDtos, typeof(IEnumerable<CandidateDto>), errorMessage);
        }

        [TestMethod]
        public void GetCandidateById_ReturnsCandidateDtoByCorrectId()
        {
            //Arrange
            int id = 1;// correct id
            var existingCandidate = GetTestCandidates().Find(c => c.Id == id);
            mockCandidateRepository.Setup(r => r.Get(id)).Returns(existingCandidate);
            mockMapper.Setup(x => x.Map<CandidateDto>(It.IsAny<Candidate>())).Returns(GetTestCandidateDtos().Find(c => c.Id == id));

            mockVacancyRepository.Setup(e => e.Get(existingCandidate.VacancyId)).Returns(new Vacancy());
            mockMapper.Setup(x => x.Map<VacancyDto>(It.IsAny<Vacancy>())).Returns(new VacancyDto());

            CandidateDto candidateDto = null;

            try
            {
                // Act
                candidateDto = candidateService.GetCandidateById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(candidateDto, errorMessage);
            Assert.IsInstanceOfType(candidateDto, typeof(CandidateDto), errorMessage);
            Assert.IsNotNull(candidateDto.VacancyDto, errorMessage);
        }

        [TestMethod]
        public void GetCandidateById_ReturnsNullByWrongId()
        {
            //Arrange
            int id = int.MaxValue - 1;// wrong id
            mockCandidateRepository.Setup(r => r.Get(t => t.Id == id)).Returns(value: null);
            CandidateDto candidateDto = null;

            try
            {
                // Act
                candidateDto = candidateService.GetCandidateById(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNull(candidateDto, errorMessage);
        }

        [TestMethod]
        public void CreateCandidate_ReturnsCandidateDto()
        {
            // Arrange scenario:
            // service recievs dto model and should map it to instance of domain type;
            var newCandidateDto = new CandidateDto() { FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 };
            mockMapper.Setup(x => x.Map<Candidate>(It.IsAny<CandidateDto>())).Returns(new Candidate());
            // pass the instance to repo, which should return model with created id:
            mockCandidateRepository.Setup(r => r.Create(new Candidate())).Returns(new Candidate()
            {
                Id = int.MaxValue,
                FullName = newCandidateDto.FullName,
                Email = newCandidateDto.Email,
                Phone = newCandidateDto.Phone,
                Notes = newCandidateDto.Notes,
                IsDismissed = newCandidateDto.IsDismissed,
                JoinedAt = newCandidateDto.JoinedAt,
                VacancyId = newCandidateDto.VacancyId
            });
            // service maps object from db back to dto type:
            mockMapper.Setup(x => x.Map<CandidateDto>(It.IsAny<Candidate>())).Returns(new CandidateDto()
            {
                Id = int.MaxValue,
                FullName = newCandidateDto.FullName,
                Email = newCandidateDto.Email,
                Phone = newCandidateDto.Phone,
                Notes = newCandidateDto.Notes,
                IsDismissed = newCandidateDto.IsDismissed,
                JoinedAt = newCandidateDto.JoinedAt,
                VacancyId = newCandidateDto.VacancyId
            });

            CandidateDto createdCandidateDto = null;

            try
            {
                // Act
                createdCandidateDto = candidateService.CreateCandidate(newCandidateDto);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(createdCandidateDto, errorMessage);
            Assert.IsInstanceOfType(createdCandidateDto, typeof(CandidateDto), errorMessage);
        }

        [TestMethod]
        public void UpdateCandidate_ReturnsUpdatedCandidateDto()
        {
            //Arrange the same scenario like in 'Create' method
            var candidateDtoToUpdate = new CandidateDto() { Id = 1, FullName = "Sindy Crowford", Email = "sindy@gmail.com", Phone = "+1234567891", Notes = "", IsDismissed = false, JoinedAt = DateTime.Today, VacancyId = 1 };
            mockMapper.Setup(x => x.Map<Candidate>(It.IsAny<CandidateDto>())).Returns(new Candidate());
            mockCandidateRepository.Setup(r => r.Update(new Candidate())).Returns(new Candidate()
            {
                Id = int.MaxValue,
                FullName = candidateDtoToUpdate.FullName,
                Email = candidateDtoToUpdate.Email,
                Phone = candidateDtoToUpdate.Phone,
                Notes = candidateDtoToUpdate.Notes,
                IsDismissed = candidateDtoToUpdate.IsDismissed,
                JoinedAt = candidateDtoToUpdate.JoinedAt,
                VacancyId = candidateDtoToUpdate.VacancyId
            });
            mockMapper.Setup(x => x.Map<CandidateDto>(It.IsAny<Candidate>())).Returns(new CandidateDto()
            {
                Id = int.MaxValue,
                FullName = candidateDtoToUpdate.FullName,
                Email = candidateDtoToUpdate.Email,
                Phone = candidateDtoToUpdate.Phone,
                Notes = candidateDtoToUpdate.Notes,
                IsDismissed = candidateDtoToUpdate.IsDismissed,
                JoinedAt = candidateDtoToUpdate.JoinedAt,
                VacancyId = candidateDtoToUpdate.VacancyId
            });

            CandidateDto updatedCandidateDto = null;

            try
            {
                // Act
                updatedCandidateDto = candidateService.UpdateCandidate(candidateDtoToUpdate);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(updatedCandidateDto, errorMessage);
            Assert.IsInstanceOfType(updatedCandidateDto, typeof(CandidateDto), errorMessage);
        }

        [TestMethod]
        public void DeleteCandidateById_ReturnsCandidateDto()
        {
            // Arrange scenario:
            // service gets id and passes it to the repo:
            int id = 1;
            mockCandidateRepository.Setup(r => r.Delete(id)).Returns(GetTestCandidates().Find(c => c.Id == id));
            // since repo.delete(int id) returns origin Candidate-object - possible to map it to dto and give it back:
            mockMapper.Setup(x => x.Map<CandidateDto>(It.IsAny<Candidate>())).Returns(GetTestCandidateDtos().Find(c => c.Id == id));

            CandidateDto candidateDto = null;

            try
            {
                // Act
                candidateDto = candidateService.DeleteCandidate(id);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message + " | " + ex.StackTrace;
            }

            //Assert
            Assert.IsNotNull(candidateDto, errorMessage);
            Assert.IsInstanceOfType(candidateDto, typeof(CandidateDto), errorMessage);
        }
    }
}
