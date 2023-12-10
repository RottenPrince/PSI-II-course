using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Question;

namespace BrainBoxUI.Tests.Controller
{
    public class LobbyControllerTests
    {
        private IApiRepository _apiRepository;

        private LobbyController _controller;
        public LobbyControllerTests()
        {
            _apiRepository = A.Fake<IApiRepository>();

            _controller = new LobbyController(_apiRepository);
        }


        [Fact]
        public void AllRooms_FetchesListOfRoomDTO_ReturnsViewWithRooms()
        {
            // Arrange
            var fakeRooms = A.Fake<List<RoomDTO>>();

            APIError? apiError;
            A.CallTo(() => _apiRepository.Get<List<RoomDTO>>(A<string>._, A<bool>._, out apiError)).Returns(fakeRooms);

            // Act
            var result = _controller.AllRooms();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeEquivalentTo(fakeRooms);
        }

    }


}
