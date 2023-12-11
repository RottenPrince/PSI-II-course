using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.Lobby;
using SharedModels.Question;
using System.Net;

namespace BrainBoxUI.Tests.Controller
{
    public class LobbyControllerTests
    {
        private IApiRepository _apiRepository;

        private LobbyController _controller;
        public LobbyControllerTests()
        {
            //Arrange
            _apiRepository = A.Fake<IApiRepository>();

            _controller = new LobbyController(_apiRepository);
        }

        [Fact]
        public void CreateRoom_ReturnsCreateView()
        {
            // Act
            var result = _controller.CreateRoom();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.Equal("Create", viewResult.ViewName);
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

        [Fact]
        public void Room_ReturnsViewWithCorrectViewBagValues()
        {
            // Arrange
            var fakeRoomId = "RoomId";
            var fakeRoomContent = new RoomContentDTO
            {
                RoomName = "Room1",
                QuestionAmount = 10
            };

            APIError? apiError;
            A.CallTo(() => _apiRepository.Get<RoomContentDTO>(A<string>._, A<bool>._, out apiError)).Returns(fakeRoomContent);

            // Act
            var result = _controller.Room(fakeRoomId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            Assert.Equal("Room1", _controller.ViewBag.RoomName);
            Assert.Equal(10, _controller.ViewBag.QuestionAmount);
            Assert.Equal("RoomId", _controller.ViewBag.RoomId);
            Assert.Null(_controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public void CreateRoom_ReturnsCreateSuccessView_WhenApiCallSucceeds()
        {
            // Arrange
             string fakeRoomName = "TestRoom";
             int fakeRoomId = 123;
            APIError? fakeError;

            A.CallTo(() => _apiRepository.Post<string, int>(A<string>._, fakeRoomName, true, out fakeError))
                .Returns(fakeRoomId);

            // Act
            var result = _controller.CreateRoom(fakeRoomName) as ViewResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<ViewResult>();
            result.ViewName.Should().Be("CreateSuccess");

            Assert.NotNull(_controller.ViewBag); 

            if (_controller.ViewBag != null)
            {
                Assert.Equal(fakeRoomName, _controller.ViewBag.RoomName);
                Assert.Equal(fakeRoomId, _controller.ViewBag.RoomId);
                Assert.Null(_controller.ViewBag.ErrorMessage); 
            }
        }

        [Fact]
        public void CreateMultiple_SetsViewBagRoomIdAndReturnsCreateMultipleView()
        {
            // Arrange

             int fakeRoomId = 123;

            // Act
            var result = _controller.CreateMultiple(fakeRoomId) as ViewResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<ViewResult>();
            result.ViewName.Should().Be("CreateMultiple");

            Assert.Equal(fakeRoomId, _controller.ViewBag.RoomId);
        }
    }
}
