using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedModels.Lobby;
using SharedModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBoxAPI.Tests.Controller
{
    public class LobbyAPIControllerTests
    {
        private IMapper _mapper;
        private IRepository<RoomModel> _roomRepo;
        private IQuizQuestionRelationRepository _relationRepo;

        private LobbyAPIController _controller;
        public LobbyAPIControllerTests()
        {
            _mapper = A.Fake<IMapper>();
            _roomRepo = A.Fake<IRepository<RoomModel>>();
            _relationRepo = A.Fake<IQuizQuestionRelationRepository>();

            _controller = new LobbyAPIController(_mapper, _roomRepo, _relationRepo);
        }

        [Fact]
        public async Task GetRoomContent_ReturnsOkResult_WithValidRoomId()
        {
            // Arrange
            var roomId = 1;

            var roomModel = new RoomModel
            {
                Id = roomId,
                Name = "TestRoom",
                Questions = new List<QuestionModel>
                {
                    new QuestionModel { Id = 1, Title = "Question 1" },
                    new QuestionModel { Id = 2, Title = "Question 2" },
                }
            };

            A.CallTo(() => _roomRepo.GetById(A<int>._))
                .Returns(Task.FromResult(roomModel));

            // Act
            var result = await _controller.GetRoomContent(roomId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeOfType<RoomContentDTO>();

            var roomContentDto = okResult.Value.Should().BeOfType<RoomContentDTO>().Subject;
            roomContentDto.Should().NotBeNull();

            roomContentDto.RoomName.Should().Be(roomModel.Name);
            roomContentDto.QuestionAmount.Should().Be(roomModel.Questions.Count);
        }

        [Fact]
        public async Task GetAllRooms_ReturnsOkResult_WithValidInput()
        {
            // Arrange
            var fakeRoomList = A.Fake<List<RoomModel>>();
            var fakeTask = Task.FromResult(fakeRoomList);

            A.CallTo(() => _roomRepo.GetAll()).Returns(fakeTask);

            var roomList = A.Fake<List<RoomDTO>>();
            A.CallTo(() => _mapper.Map<List<RoomDTO>>(fakeRoomList)).Returns(roomList);

            // Act
            var result = await _controller.GetAllRooms();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }


        //    PROBLEM WITH .SAVE
        //    [Fact]
        //    public async Task CreateRoom_ReturnsOkResult_WithValidInput()
        //    {
        //        // Arrange
        //        var roomName = "TestRoom";

        //        var newRoom = new RoomModel { Name = roomName };
        //        A.CallTo(() => _roomRepo.Add(A<RoomModel>._)).DoesNothing();

        //        A.CallTo(() => _roomRepo.Save()).WithAnyArguments().Invokes(call =>
        //        {
        //            // Simulate the behavior of assigning the generated Id to newRoom.Id
        //            newRoom.Id = 1;
        //        });

        //        // Act
        //        var result = await _controller.CreateRoom(roomName);

        //        // Assert
        //        result.Should().BeOfType<OkObjectResult>();

        //        var okResult = result as OkObjectResult;
        //        okResult.Should().NotBeNull();

        //        okResult.Value.Should().BeOfType<int>();

        //        var roomId = (int)okResult.Value;
        //        roomId.Should().BeGreaterThan(0);
        //        roomId.Should().Be(newRoom.Id);
        //    }




        /*
        [Fact]
        public async Task GetNextQuestionInQuiz_ReturnsOkResultWithQuestionDTO()
        {
            // Arrange
            var mapperMock = new Mock<IMapper>();
            var roomRepoMack = new Mock<IRepository<RoomModel>>();
            var relationRepoMack = new Mock<IQuizQuestionRelationRepository>();
            var controller = new LobbyAPIController(_mapper, _roomRepo, _relationRepo);

            var questionModel = new QuestionModel { Id = 1, Title = "test 1" };
            var quizRelationModel = new QuizQuestionRelationModel { Question = questionModel };
            relationRepoMack.Setup(repo => repo.GetNextQuestionInQuiz(123)).ReturnsAsync(quizRelationModel);
            mapperMock.Setup(mapper => mapper.Map<QuestionDTO>(questionModel))
                .Returns(new QuestionDTO { Id = 1, Title = "test 1" });

            // Act
            var result = await controller.GetNextQuestionInQuiz(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var questionDTO = Assert.IsType<QuestionDTO>(okResult.Value);
            Assert.Equal(1, questionDTO.Id);
            Assert.Equal("test 1", questionDTO.Title);
        */

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

        [Fact]
        public async Task CreateQuiz_ReturnsOkResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.CreateNewQuiz(A<int>._, A<int>._)).Returns(1);

            // Act
            var result = await _controller.CreateQuiz(1, 5) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(1, result.Value);
            // Add more assertions as needed
        }

        [Fact]
        public async Task CreateQuiz_ReturnsNotFoundResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.CreateNewQuiz(A<int>._, A<int>._)).Returns(-1);

            // Act
            var result = await _controller.CreateQuiz(1, 5) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            // Add more assertions as needed
        }


        [Fact]
        public async Task GetAllQuizQuestionsInfo_ReturnsOkResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(A<int>._)).Returns(Task.FromResult(new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel() }
            }));

            // Act
            var result = await _controller.GetAllQuizQuestionsInfo(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetNextQuestionInReview_ReturnsOkResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.GetNextQuestionInReview(A<int>._, A<int>._)).Returns(Task.FromResult(new QuizQuestionRelationModel()));

            // Act
            var result = await _controller.GetNextQuestionInReview(1, 0) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetAllQuizQuestionsInfo_ReturnsUnauthorizedResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(A<int>._)).Returns(Task.FromResult(new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = null }
            }));

            // Act
            var result = await _controller.GetAllQuizQuestionsInfo(1) as UnauthorizedResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetNextQuestionInReview_ReturnsNoContentResult()
        {
            // Arrange
            A.CallTo(() => _relationRepo.GetNextQuestionInReview(A<int>._, A<int>._)).Returns(Task.FromResult<QuizQuestionRelationModel>(null));

            // Act
            var result = await _controller.GetNextQuestionInReview(1, 0) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
            // Add more assertions as needed
        }

    }
}
