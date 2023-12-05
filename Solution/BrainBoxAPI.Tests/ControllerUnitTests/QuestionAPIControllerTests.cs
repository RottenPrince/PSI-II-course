using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.Question;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Caching;
using SharedModels.Lobby;

namespace BrainBoxAPI.Tests.ControllerUnitTests
{
    public class QuestionAPIControllerTests
    {
        private ILogger<QuestionAPIController> _logger;
        private IMapper _mapper;
        private IRepository<QuestionModel> _questionRepo;
        private IDictionaryCache<int, RoomContentDTO> _cache;

        private QuestionAPIController _controller;

        public QuestionAPIControllerTests()
        {
            _logger = A.Fake<ILogger<QuestionAPIController>>();
            _mapper = A.Fake<IMapper>();
            _questionRepo = A.Fake<IRepository<QuestionModel>>();
            _cache = A.Fake<IDictionaryCache<int, RoomContentDTO>>();

            _controller = new QuestionAPIController(_logger, _mapper, _questionRepo, _cache);
        }

        [Fact]
        public async Task GetQuestion_ReturnsOkResult_WithValidId()
        {
            // Arrange
            var questionId = 1;

            var questionModel = new QuestionModel
            {
                Id = 1,
                Title = "Just Question",
                ImageSource = null,
            };

            A.CallTo(() => _questionRepo.GetById(A<int>._))
                .Returns(Task.FromResult(questionModel));

            var expectedDto = new QuestionDTO
            {
                Id = questionModel.Id,
                Title = questionModel.Title,
                ImageSource = questionModel.ImageSource,
            };
            A.CallTo(() => _mapper.Map<QuestionDTO>(A<QuestionModel>._))
                .Returns(expectedDto);

            // Act
            var result = await _controller.GetQuestion(questionId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();


            okResult.Value.Should().NotBeNull();

            var returnedDto = okResult.Value.Should().BeOfType<QuestionDTO>().Subject;
            returnedDto.Should().BeEquivalentTo(expectedDto);
            returnedDto.Id.Should().Be(questionModel.Id);
            returnedDto.Title.Should().Be(questionModel.Title);
            returnedDto.ImageSource.Should().Be(questionModel.ImageSource);
            A.CallTo(() => _questionRepo.GetById(questionId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _mapper.Map<QuestionDTO>(questionModel)).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task GetFullQuestion_ReturnsOkResult_WithValidId()
        {
            // Arrange
            var questionId = 1;

            var questionModel = new QuestionModel
            {
                Id = 1,
                Title = "Just Question",
                ImageSource = null
            };

            A.CallTo(() => _questionRepo.GetById(A<int>._))
                .Returns(Task.FromResult(questionModel));

            var expectedDto = new QuestionWithAnswerDTO
            {
                Id = questionModel.Id,
                Title = questionModel.Title,
                ImageSource = questionModel.ImageSource,
            };
            A.CallTo(() => _mapper.Map<QuestionWithAnswerDTO>(A<QuestionModel>._))
                .Returns(expectedDto);

            // Act
            var result = await _controller.GetFullQuestion(questionId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().NotBeNull();

            var returnedDto = okResult.Value.Should().BeOfType<QuestionWithAnswerDTO>().Subject;
            returnedDto.Should().BeEquivalentTo(expectedDto);
            returnedDto.Id.Should().Be(questionModel.Id);
            returnedDto.Title.Should().Be(questionModel.Title);
            returnedDto.ImageSource.Should().Be(questionModel.ImageSource);
            A.CallTo(() => _questionRepo.GetById(questionId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _mapper.Map<QuestionWithAnswerDTO>(questionModel)).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task GetFullQuestion_ReturnsOkResult_WithNotValidId()
        {
            var questionId = 1;
            // Arrange
            A.CallTo(() => _questionRepo.GetById(A<int>._))
        .Returns(Task.FromResult<QuestionModel>(null));

            // Act
            var notFoundResult = await _controller.GetFullQuestion(questionId);

            // Assert
            notFoundResult.Should().BeOfType<NotFoundResult>();
        }

    }
}
