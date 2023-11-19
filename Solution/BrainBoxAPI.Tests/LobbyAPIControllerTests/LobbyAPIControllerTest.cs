using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Exceptions;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio;
using Moq;
using SharedModels.Lobby;
using SharedModels.Question;

namespace BrainBoxAPI.Tests.LobbyAPIControllerTests
{
    public class LobbyAPIControllerTests
    {
        private Mock<IRepository<RoomModel>> _mockRoomRepo;
        private Mock<IQuizQuestionRelationRepository> _mockRelationRepo;
        private Mock<IMapper> _mockMapper;

        [Fact]
        public void Setup()
        {
            _mockRoomRepo = new Mock<IRepository<RoomModel>>();
            _mockRelationRepo = new Mock<IQuizQuestionRelationRepository>();
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllRooms_ReturnsOkResult()
        {
            // Arrange
            var controller = new LobbyAPIController(_mockMapper.Object, _mockRoomRepo.Object, _mockRelationRepo.Object);
            _mockRoomRepo.Setup(repo => repo.GetAll()).ReturnsAsync(new List<RoomModel>());

            // Act
            var result = await controller.GetAllRooms() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetRoomContent_ReturnsOkResult()
        {
            // Arrange
            var controller = new LobbyAPIController(_mockMapper.Object, _mockRoomRepo.Object, _mockRelationRepo.Object);
            _mockRoomRepo.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(new RoomModel { Questions = new List<QuestionModel>() });

            // Act
            var result = await controller.GetRoomContent(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task CreateRoom_ReturnsOkResult()
        {
            // Arrange
            var controller = new LobbyAPIController(_mockMapper.Object, _mockRoomRepo.Object, _mockRelationRepo.Object);
            _mockRoomRepo.Setup(repo => repo.Save());

            // Act
            var result = await controller.CreateRoom("Room1") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task CreateQuiz_ReturnsOkResultWithQuizId()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            relationRepoMock.Setup(repo => repo.CreateNewQuiz(1, 5)).ReturnsAsync(123);

            // Act
            var result = await controller.CreateQuiz(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var quizId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(123, quizId);
        }

        [Fact]
        public async Task GetNextQuestionInQuiz_ReturnsOkResultWithQuestionDTO()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            var questionModel = new QuestionModel { Id = 1, Title = "Sample Question" };
            var quizRelationModel = new QuizQuestionRelationModel { Question = questionModel };
            relationRepoMock.Setup(repo => repo.GetNextQuestionInQuiz(123)).ReturnsAsync(quizRelationModel);
            mapperMock.Setup(mapper => mapper.Map<QuestionDTO>(questionModel))
                .Returns(new QuestionDTO { Id = 1, Title = "Sample Question" });

            // Act
            var result = await controller.GetNextQuestionInQuiz(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var questionDTO = Assert.IsType<QuestionDTO>(okResult.Value);
            Assert.Equal(1, questionDTO.Id);
            Assert.Equal("Sample Question", questionDTO.Title);
        }

        [Fact]
        public async Task SubmitAnswer_ReturnsOkResult()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            var quizRelationModel = new QuizQuestionRelationModel { Question = new QuestionModel(), SelectedAnswerOption = new AnswerOptionModel() };
            relationRepoMock.Setup(repo => repo.GetNextQuestionInQuiz(123)).ReturnsAsync(quizRelationModel);

            // Act
            var result = await controller.SubmitAnswer(123, 1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetAllQuizQuestionsInfo_ReturnsOkResultWithQuizQuestionsDTO()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            var quizRelationModels = new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel(), Question = new QuestionModel() }
            };

            relationRepoMock.Setup(repo => repo.GetAllQuizQuestionsInfo(123)).ReturnsAsync(quizRelationModels);
            mapperMock.Setup(mapper => mapper.Map<List<QuizQuestionDTO>>(quizRelationModels))
                .Returns(new List<QuizQuestionDTO> { new QuizQuestionDTO() });

            // Act
            var result = await controller.GetAllQuizQuestionsInfo(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var quizQuestionsDTO = Assert.IsType<List<QuizQuestionDTO>>(okResult.Value);
            Assert.Single(quizQuestionsDTO);
        }

        [Fact]
        public async Task GetNextQuestionInReview_ReturnsOkResultWithQuizQuestionDTO()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            var quizRelationModel = new QuizQuestionRelationModel { Question = new QuestionModel(), SelectedAnswerOption = new AnswerOptionModel() };
            relationRepoMock.Setup(repo => repo.GetNextQuestionInReview(123, 1)).ReturnsAsync(quizRelationModel);
            mapperMock.Setup(mapper => mapper.Map<QuizQuestionDTO>(quizRelationModel))
                .Returns(new QuizQuestionDTO());

            // Act
            var result = await controller.GetNextQuestionInReview(123, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var quizQuestionDTO = Assert.IsType<QuizQuestionDTO>(okResult.Value);
            Assert.NotNull(quizQuestionDTO);
        }


        [Fact]
        public async Task GetRoomId_ReturnsOkResultWithRoomId()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMock = new Mock<IRepository<RoomModel>>();
            var relationRepoMock = new Mock<IQuizQuestionRelationRepository>();

            var controller = new LobbyAPIController(mapperMock.Object, roomRepoMock.Object, relationRepoMock.Object);

            var quizRelationModels = new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel(), Question = new QuestionModel { RoomId = 456 } }
            };

            relationRepoMock.Setup(repo => repo.GetAllQuizQuestionsInfo(123)).ReturnsAsync(quizRelationModels);

            // Act
            var result = await controller.GetRoomId(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roomId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(456, roomId);
        }


    }
}
