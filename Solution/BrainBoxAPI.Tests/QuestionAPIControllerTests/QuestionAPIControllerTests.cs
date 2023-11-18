using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.Question;
using BrainBoxAPI.Managers;

namespace BrainBoxAPI.Tests
{
    public class QuestionAPIControllerTests
    {
        [Fact]
        public async Task GetFullQuestion_ReturnsOkResult_WithValidId()
        {
            // Arrange
            var questionId = 1; 

            var questionModel = new QuestionModel
            {
                Id = 1,
                Title = "Just Question",
                ImageSource = null,
            };

            var questionRepoFake = A.Fake<IRepository<QuestionModel>>();
            A.CallTo(() => questionRepoFake.GetById(A<int>._))
                .Returns(Task.FromResult(questionModel));

            var mapperFake = A.Fake<IMapper>();
            A.CallTo(() => mapperFake.Map<QuestionDTO>(A<QuestionModel>._))
                .Returns(new QuestionDTO
                {
                    Id = questionModel.Id,
                    Title = questionModel.Title,
                    ImageSource = questionModel.ImageSource
                });

            var loggerFake = A.Fake<ILogger<QuestionAPIController>>();

            var controller = new QuestionAPIController(loggerFake, mapperFake, questionRepoFake);

            // Act
            var result = await controller.GetFullQuestion(questionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<QuestionDTO>(okResult.Value);

            Assert.Equal(questionModel.Title, model.Title);
        }
    }
}
