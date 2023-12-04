using AutoMapper;
using BrainBoxAPI.Caching;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Lobby;
using SharedModels.Question;
using System.Net;

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
            var fakeRoomList = new List<RoomModel>
            {
                new RoomModel { Id = 1, Name = "Room 1" },
                new RoomModel { Id = 2, Name = "Room 2" },
            };
            var fakeTask = Task.FromResult(fakeRoomList);

            A.CallTo(() => _roomRepo.GetAll()).Returns(fakeTask);

            var roomDTOs = fakeRoomList.Select(roomModel => new RoomDTO { Id = roomModel.Id, Name = roomModel.Name }).ToList();
            A.CallTo(() => _mapper.Map<List<RoomDTO>>(fakeRoomList)).Returns(roomDTOs);

            // Act
            var result = await _controller.GetAllRooms();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

           
            var okObjectResult = result.As<OkObjectResult>();
            okObjectResult.Should().NotBeNull();
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var returnedRoomList = okObjectResult.Value.As<List<RoomDTO>>();
            returnedRoomList.Should().NotBeNull();

            returnedRoomList.Should().HaveCount(fakeRoomList.Count);

            for (int i = 0; i < returnedRoomList.Count; i++)
            {
                returnedRoomList[i].Id.Should().Be(fakeRoomList[i].Id);
                returnedRoomList[i].Name.Should().Be(fakeRoomList[i].Name);
            }
            A.CallTo(() => _mapper.Map<List<RoomDTO>>(fakeRoomList)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _roomRepo.GetAll()).MustHaveHappenedOnceExactly();
        }


        [Fact]
        public async Task CreateRoom_ReturnsOkResult_WithValidInput()
        {
            // Arrange
            var roomName = "TestRoom";

            var newRoom = new RoomModel { Name = roomName};
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
            A.CallTo(() => _roomRepo.Add(A<RoomModel>.That.Matches(r => r.Name == roomName))).MustHaveHappenedOnceExactly();
            A.CallTo(() => _roomRepo.Save()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _cache.Invalidate(newRoom.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetAllQuizQuestionsInfo_ReturnsOkResultWithQuizQuestionsDTO()
        {
            // Arrange
            var quizRelationModels = new List<QuizQuestionRelationModel>
            {
                new QuizQuestionRelationModel
                {
                    Id = 1,
                    QuizModelID = 123,
                    QuestionModelID = 456,
                    SelectedAnswerOption = new AnswerOptionModel
                    {
                        Id = 789,
                        OptionText = "Option 1",
                        IsCorrect = true,
                        QuestionId = 456
                    },
                    Question = new QuestionModel
                    {
                        Id = 456,
                        Title = "Question 1",
                        ImageSource = "image.jpg",
                        RoomId = 789
                    },
                    Quiz = new QuizModel
                    {
                        Id = 123,
                        RoomId = 789,
                        StartTime = DateTime.Now
                    }
                }
            };

            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(123))
                .Returns(Task.FromResult(quizRelationModels));

            var expectedQuizQuestionDTOs = quizRelationModels.Select(q => new QuizQuestionDTO
            {
                Question = new QuestionWithAnswerDTO
                {
                    Id = q.Question.Id,
                    Title = q.Question.Title,
                    ImageSource = q.Question.ImageSource,
                    CorrectAnswerIndex = 0,
                },
                SelectedAnswerOption = 0,
            }).ToList();

            A.CallTo(() => _mapper.Map<List<QuizQuestionDTO>>(A<IEnumerable<QuizQuestionRelationModel>>._))
                .Returns(expectedQuizQuestionDTOs);

            // Act
            var result = await _controller.GetAllQuizQuestionsInfo(123);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var quizQuestionsDTO = okResult.Value.Should().BeAssignableTo<List<QuizQuestionDTO>>().Subject;

            quizQuestionsDTO.Should().HaveCount(1);

            var expectedQuizQuestionDTO = expectedQuizQuestionDTOs.First();
            var actualQuizQuestionDTO = quizQuestionsDTO.First();

            actualQuizQuestionDTO.Question.Id.Should().Be(expectedQuizQuestionDTO.Question.Id);
            actualQuizQuestionDTO.Question.Title.Should().Be(expectedQuizQuestionDTO.Question.Title);
            actualQuizQuestionDTO.Question.ImageSource.Should().Be(expectedQuizQuestionDTO.Question.ImageSource);
            actualQuizQuestionDTO.Question.CorrectAnswerIndex.Should().Be(expectedQuizQuestionDTO.Question.CorrectAnswerIndex);
            actualQuizQuestionDTO.SelectedAnswerOption.Should().Be(expectedQuizQuestionDTO.SelectedAnswerOption);
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
            int expectedRunId = 123;
            int expectedRoomId = 456;

            var quizRelationModels = new List<QuizQuestionRelationModel>
    {
        new QuizQuestionRelationModel { SelectedAnswerOption = new AnswerOptionModel(), Question = new QuestionModel { RoomId = expectedRoomId } }
    };

            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(expectedRunId)).Returns(Task.FromResult(quizRelationModels));

            // Act
            var result = await _controller.GetRoomId(expectedRunId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.IsType<int>(okResult.Value);

            var roomId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(expectedRoomId, roomId);
            Assert.Equal(200, okResult.StatusCode);
            A.CallTo(() => _relationRepo.GetAllQuizQuestionsInfo(expectedRunId)).MustHaveHappenedOnceExactly();
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
