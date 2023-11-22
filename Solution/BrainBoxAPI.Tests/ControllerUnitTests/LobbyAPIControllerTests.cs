using AutoMapper;
using BrainBoxAPI.Caching;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Lobby;
using SharedModels.Question;

namespace BrainBoxAPI.Tests.ControllerUnitTests
{
    public class LobbyAPIControllerTests
    {
        private IMapper _mapper;
        private IRepository<RoomModel> _roomRepo;
        private IQuizQuestionRelationRepository _relationRepo;
        private IDictionaryCache<int, RoomContentDTO> _cache;

        private LobbyAPIController _controller;
        public LobbyAPIControllerTests()
        {
            _mapper = A.Fake<IMapper>();
            _roomRepo = A.Fake<IRepository<RoomModel>>();
            _relationRepo = A.Fake<IQuizQuestionRelationRepository>();
            _cache = A.Fake<IDictionaryCache<int, RoomContentDTO>>();

            _controller = new LobbyAPIController(_mapper, _roomRepo, _relationRepo, _cache);
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


        [Fact]
        public async Task CreateRoom_ReturnsOkResult_WithValidInput()
        {
            // Arrange
            var roomName = "TestRoom";

            var newRoom = new RoomModel { Name = roomName };
            A.CallTo(() => _roomRepo.Add(A<RoomModel>._)).DoesNothing();

            A.CallTo(() => _roomRepo.Save()).Returns(1); 

            // Act
            var result = await _controller.CreateRoom(roomName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeOfType<int>();

            var roomId = (int)okResult.Value;

            newRoom.Id.Should().Be(roomId);
        }

        [Fact]
        public async Task GetAllQuizQuestionsInfo_ReturnsOkResultWithQuizQuestionsDTO()
        {
            // Arrange

            var quizRelationModels = new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel(), Question = new QuestionModel() }
            };

            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(123))
                .Returns(Task.FromResult(quizRelationModels));

            A.CallTo(() => _mapper.Map<List<QuizQuestionDTO>>(A<IEnumerable<QuizQuestionRelationModel>>._))
                .Returns(new List<QuizQuestionDTO> { new QuizQuestionDTO() });

            // Act
            var result = await _controller.GetAllQuizQuestionsInfo(123);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var quizQuestionsDTO = Assert.IsType<List<QuizQuestionDTO>>(okResult.Value);
            Assert.Single(quizQuestionsDTO);
        }

        [Fact]
        public async Task GetNextQuestionInReview_ReturnsOkResultWithQuizQuestionDTO()
        {
            // Arrange
            var quizRelationModel = new QuizQuestionRelationModel { Question = new QuestionModel(), SelectedAnswerOption = new AnswerOptionModel() };

            A.CallTo(() => _relationRepo.GetNextQuestionInReview(123, 1))
                .ReturnsLazily(() => Task.FromResult<QuizQuestionRelationModel?>(quizRelationModel));

            A.CallTo(() => _mapper.Map<QuizQuestionDTO>(A<QuizQuestionRelationModel>._))
                .ReturnsLazily(() => new QuizQuestionDTO());

            // Act
            var result = await _controller.GetNextQuestionInReview(123, 1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var quizQuestionDTO = Assert.IsType<QuizQuestionDTO>(okResult.Value);
            Assert.NotNull(quizQuestionDTO);
        }

        [Fact]
        public async Task GetRoomId_ReturnsOkResultWithRoomId()
        {
            // Arrange
            var quizRelationModels = new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel(), Question = new QuestionModel { RoomId = 456 } }
            };

            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(123)).Returns(Task.FromResult(quizRelationModels));

            // Act
            var result = await _controller.GetRoomId(123);

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

        }

    }
}
