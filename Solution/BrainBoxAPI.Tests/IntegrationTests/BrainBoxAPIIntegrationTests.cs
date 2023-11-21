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
using SharedModels.Lobby;
using SharedModels.Question;


namespace BrainBoxAPI.Tests.IntegrationTests
{
    public class BrainBoxAPIIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public BrainBoxAPIIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllRooms_ReturnsSuccessAndRooms()
        {
            // Act
            var response = await _client.GetAsync("/api/Lobby/GetAllRooms");
            response.EnsureSuccessStatusCode();

            var rooms = await JsonSerializer.DeserializeAsync<List<RoomDTO>>(await response.Content.ReadAsStreamAsync());

            // Assert
            Assert.NotNull(rooms);
            Assert.NotEmpty(rooms);
        }

        [Fact]
        public async Task GetRoomContent_ReturnsRoomContent()
        {
            // Arrange

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                dbContext.SeedTestData();
            }

            // Act
            var response = await _client.GetAsync($"/api/Lobby/GetRoomContent/1");
            response.EnsureSuccessStatusCode();

            var roomContent = await response.Content.ReadFromJsonAsync<RoomContentDTO>();

            // Assert
            Assert.NotNull(roomContent);
            Assert.Equal("Room 1", roomContent.RoomName);
            Assert.Equal(2, roomContent.QuestionAmount);
        }

    }
}
