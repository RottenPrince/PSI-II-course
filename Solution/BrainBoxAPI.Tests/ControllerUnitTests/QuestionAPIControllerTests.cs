using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.Question;
using BrainBoxAPI.Managers;
using SharedModels.Lobby;

namespace BrainBoxAPI.Tests.ControllerUnitTests
{
    public class QuestionAPIControllerTests
    {
        private ILogger<QuestionAPIController> _logger;
        private IMapper _mapper;
        private IRepository<QuestionModel> _questionRepo;

        private QuestionAPIController _controller;

        public QuestionAPIControllerTests()
        {
            _logger = A.Fake<ILogger<QuestionAPIController>>();
            _mapper = A.Fake<IMapper>();
            _questionRepo = A.Fake<IRepository<QuestionModel>>();

            _controller = new QuestionAPIController(_logger, _mapper, _questionRepo);
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
        }




    }
}
