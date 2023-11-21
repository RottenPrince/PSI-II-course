using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BrainBoxAPI;
using BrainBoxAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using SharedModels.Question;

namespace BrainBoxAPI.Tests.IntegrationTests
{
    public class BrainBoxAPIIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;

        public BrainBoxAPIIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private async Task<HttpResponseMessage> PostQuestionAsync(QuestionWithAnswerDTO question)
        {
            var client = _factory.CreateClient();

            // Simulate the behavior of _apiRepository.Post
            var questionJson = JsonSerializer.Serialize(question);
            var content = new StringContent(questionJson, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Question/SaveQuestion/1?image=null", content);

            return response;
        }

        [Fact]
        public async Task SaveQuestion_ReturnsSuccessStatusCode()
        {
            // Arrange
            var question = new QuestionWithAnswerDTO
            {
                Id = 0,
                Title = "Sample Question",
                AnswerOptions = new List<AnswerOptionDTO>
                {
                    new AnswerOptionDTO { Id = 1, OptionText = "Option 1" },
                    new AnswerOptionDTO { Id = 2, OptionText = "Option 2" },
                },
                ImageSource = "sample-image.jpg",
                CorrectAnswerIndex = 0
            };

            // Act
            var response = await PostQuestionAsync(question);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
