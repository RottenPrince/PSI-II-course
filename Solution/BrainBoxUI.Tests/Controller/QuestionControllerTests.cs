using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.Question;
using System.Net;


namespace BrainBoxUI.Tests.Controller
{
    public class QuestionControllerTests
    {
        private IApiRepository _apiRepository;
        private IWebHostEnvironment _host;
        private readonly ILogger<QuestionController> _logger;

        private QuestionController _controller;
        public QuestionControllerTests()
        {
            _host = A.Fake<IWebHostEnvironment>(); ; ;
            _logger = A.Fake<ILogger<QuestionController>>(); ;
            _apiRepository = A.Fake<IApiRepository>();

            _controller = new QuestionController(_host, _logger, _apiRepository);
        }
        [Fact]
        public void Create_ReturnsViewWithQuestionModel()
        {
            // Arrange
            var roomId = "RoomId";

            // Act
            var result = _controller.Create(roomId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.ViewName);

            var model = result.Model as QuestionWithAnswerDTO;
            Assert.NotNull(model);
            Assert.Equal(roomId, result.ViewData["RoomId"]);
        }
        [Fact]
        public void StartRun_ReturnsRedirectToSolve_WhenApiCallSucceeds()
        {
            // Arrange
             int fakeRoomId = 123;
             int fakeQuestionAmount = 5;
             int fakeRunId = 676;

            APIError? fakeError;

            A.CallTo(() => _apiRepository.Get<int>(A<string>._, A<bool>._, out fakeError))
                .Returns(fakeRunId);

            // Act
            var result = _controller.StartRun(fakeRoomId, fakeQuestionAmount) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Solve");
            result.RouteValues.Should().ContainKey("runId");
            result.RouteValues["runId"].Should().Be(fakeRunId);

        }

        

        [Fact]
        public void Solve_ReturnsRedirectToReview_WhenNoNextQuestion()
        {
            // Arrange
            int fakeRunId = 123;
            int fakeCurrentQuestionIndex = 0;

            APIError? fakeError;
            A.CallTo(() => _apiRepository.Get<QuestionDTO>(A<string>._, A<bool>._, out fakeError))
                .Returns(null); 

            // Act
            var result = _controller.Solve(fakeRunId) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Review");
            result.RouteValues.Should().ContainKey("runId");
            result.RouteValues["runId"].Should().Be(fakeRunId);
            result.RouteValues.Should().ContainKey("currentQuestionIndex");
            result.RouteValues["currentQuestionIndex"].Should().Be(fakeCurrentQuestionIndex);
        }

        [Fact]
        public void Solve_ReturnsSolveView_WhenNextQuestionExists()
        {
            // Arrange
            int fakeRunId = 123;

            var fakeQuestionModel = new QuestionDTO
            {
                Id = 1,
                Title = "It is a Question",
                AnswerOptions = new List<AnswerOptionDTO>
        {
            new AnswerOptionDTO { Id = 1, OptionText = "Option a" },
            new AnswerOptionDTO { Id = 2, OptionText = "Option b" },
        },
                ImageSource = "just-an-image.jpg"
            };
            APIError? fakeError;

            A.CallTo(() => _apiRepository.Get<QuestionDTO>(A<string>._, A<bool>._, out fakeError))
                .Returns(fakeQuestionModel);

            // Act
            var result = _controller.Solve(fakeRunId) as ViewResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<ViewResult>();
            result.ViewName.Should().Be("Solve");

            result.ViewData.Model.Should().BeOfType<QuestionDTO>();
            var model = result.ViewData.Model as QuestionDTO;
            model.Id.Should().Be(fakeQuestionModel.Id);
            model.Title.Should().Be(fakeQuestionModel.Title);
            model.AnswerOptions.Should().BeEquivalentTo(fakeQuestionModel.AnswerOptions);
            model.ImageSource.Should().Be(fakeQuestionModel.ImageSource);

        }
        [Fact]
        public void Solve_PostsToSubmitAnswerAndRedirectsToSolve()
        {
            // Arrange
            int fakeRunId = 123;
            int fakeSelectedOption = 1;

            APIError? fakeError;

            A.CallTo(() => _apiRepository.Post<object, string>(A<string>._, A<object>._, A<bool>._, out fakeError))
                .Returns("Success"); 

            // Act
            var result = _controller.Solve(fakeRunId, fakeSelectedOption) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Solve");
            result.RouteValues.Should().ContainKey("runId");
            result.RouteValues["runId"].Should().Be(fakeRunId);
        }

        [Fact]
        public void Review_RedirectsToRoom_WhenNextQuestionInReviewIsNull()
        {
            // Arrange
            int fakeRunId = 123;
            int fakeCurrentQuestionIndex = 1;
            QuizQuestionDTO fakeQuestionModel = null;

            APIError? fakeError;

            A.CallTo(() => _apiRepository.Get<QuizQuestionDTO>(A<string>._, A<bool>._, out fakeError))
                .Returns(fakeQuestionModel);

            int fakeRoomId = 456;
            A.CallTo(() => _apiRepository.Get<int>(A<string>._, A<bool>._, out fakeError))
                .Returns(fakeRoomId);

            // Act
            var result = _controller.Review(fakeRunId, fakeCurrentQuestionIndex) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Room");
            result.ControllerName.Should().Be("Lobby");
            result.RouteValues["roomId"].Should().Be(fakeRoomId);
        }



        [Fact]
        public void Review_ReturnsReviewView_WhenNextQuestionInReviewExists()
        {
            // Arrange
            int fakeRunId = 123;
            int fakeCurrentQuestionIndex = 1;

            var fakeAnswerOptions = new List<AnswerOptionDTO>
    {
        new AnswerOptionDTO { Id = 1, OptionText = "Option a" },
        new AnswerOptionDTO { Id = 2, OptionText = "Option b" },
    };

            var fakeQuestionWithAnswerDTO = new QuestionWithAnswerDTO
            {
                Id = 1,
                Title = "It is a Question",
                AnswerOptions = fakeAnswerOptions,
                CorrectAnswerIndex = 2
            };

            var fakeQuizQuestionDTO = new QuizQuestionDTO
            {
                Question = fakeQuestionWithAnswerDTO,
                SelectedAnswerOption = 2 
            };

            APIError? fakeError;

            A.CallTo(() => _apiRepository.Get<QuizQuestionDTO>(A<string>._, A<bool>._, out fakeError))
                .Returns(fakeQuizQuestionDTO);

            // Act
            var result = _controller.Review(fakeRunId, fakeCurrentQuestionIndex) as ViewResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<ViewResult>();
            result.ViewName.Should().Be("Review");

            result.ViewData["runId"].Should().Be(fakeRunId);
            result.ViewData["currentQuestionIndex"].Should().Be(fakeCurrentQuestionIndex + 1);

            result.Model.Should().NotBeNull().And.BeOfType<QuizQuestionDTO>();
            var model = result.Model as QuizQuestionDTO;

            model.Question.Should().NotBeNull().And.BeOfType<QuestionWithAnswerDTO>();
            var questionModel = model.Question as QuestionWithAnswerDTO;
            questionModel.Should().BeEquivalentTo(fakeQuestionWithAnswerDTO);

            questionModel.AnswerOptions.Should().NotBeNull().And.BeEquivalentTo(fakeAnswerOptions);
            model.SelectedAnswerOption.Should().Be(fakeQuizQuestionDTO.SelectedAnswerOption);

            questionModel.Id.Should().Be(fakeQuestionWithAnswerDTO.Id);
            questionModel.Title.Should().Be(fakeQuestionWithAnswerDTO.Title);
            questionModel.CorrectAnswerIndex.Should().Be(fakeQuestionWithAnswerDTO.CorrectAnswerIndex);

        }
    }
}