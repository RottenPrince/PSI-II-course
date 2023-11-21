using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBoxUI.Tests.Controller
{
    public class QuestionControllerTests
    {
        private IApiRepository _apiRepository;
        private  IWebHostEnvironment _host;
        private readonly ILogger<QuestionController> _logger;

        private QuestionController _controller;
        public QuestionControllerTests()
        {
            _host = A.Fake<IWebHostEnvironment>(); ; ;
            _logger = A.Fake<ILogger<QuestionController>>(); ;
            _apiRepository = A.Fake<IApiRepository>();

            _controller = new QuestionController(_host, _logger, _apiRepository );
        }
        [Fact]
        public void Create_ReturnsViewWithQuestionModel()
        {
            // Arrange
            var roomId = "testRoomId";

            // Act
            var result = _controller.Create(roomId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.ViewName); 

            var model = result.Model as QuestionWithAnswerDTO;
            Assert.NotNull(model);
            Assert.Equal(roomId, result.ViewData["RoomId"]);
        }
    }
}
