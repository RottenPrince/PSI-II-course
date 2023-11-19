using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBoxAPI.Tests.Controller
{
    public class LobbyAPIControllerTests
    {
        private IMapper _mapper;
        private IRepository<RoomModel> _roomRepo;
        private IQuizQuestionRelationRepository _relationRepo;

        private LobbyAPIController _controller;
        public LobbyAPIControllerTests()
        {
            _mapper = A.Fake<IMapper>();
            _roomRepo = A.Fake<IRepository<RoomModel>>();
            _relationRepo = A.Fake<IQuizQuestionRelationRepository>();

            _controller = new LobbyAPIController(_mapper, _roomRepo, _relationRepo);
        }

        [Fact]
        public async Task GetRoomContent_ReturnsOkResult_WithValidRoomId()
        {
            // Arrange
            var roomId = 1;

            var roomModel = new RoomModel
            {
                Id = roomId,
                Name = "TestRoom",
                Questions = new List<QuestionModel>
                {
                    new QuestionModel { Id = 1, Title = "Question 1" },
                    new QuestionModel { Id = 2, Title = "Question 2" },
                }
            };

            A.CallTo(() => _roomRepo.GetById(A<int>._))
                .Returns(Task.FromResult(roomModel));

            // Act
            var result = await _controller.GetRoomContent(roomId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            okResult.Value.Should().BeOfType<RoomContentDTO>();

            var roomContentDto = okResult.Value.Should().BeOfType<RoomContentDTO>().Subject;
            roomContentDto.Should().NotBeNull();

            roomContentDto.RoomName.Should().Be(roomModel.Name);
            roomContentDto.QuestionAmount.Should().Be(roomModel.Questions.Count);
        }


        //    PROBLEM WITH .SAVE
        //    [Fact]
        //    public async Task CreateRoom_ReturnsOkResult_WithValidInput()
        //    {
        //        // Arrange
        //        var roomName = "TestRoom";

        //        var newRoom = new RoomModel { Name = roomName };
        //        A.CallTo(() => _roomRepo.Add(A<RoomModel>._)).DoesNothing();

        //        A.CallTo(() => _roomRepo.Save()).WithAnyArguments().Invokes(call =>
        //        {
        //            // Simulate the behavior of assigning the generated Id to newRoom.Id
        //            newRoom.Id = 1;
        //        });

        //        // Act
        //        var result = await _controller.CreateRoom(roomName);

        //        // Assert
        //        result.Should().BeOfType<OkObjectResult>();

        //        var okResult = result as OkObjectResult;
        //        okResult.Should().NotBeNull();

        //        okResult.Value.Should().BeOfType<int>();

        //        var roomId = (int)okResult.Value;
        //        roomId.Should().BeGreaterThan(0);
        //        roomId.Should().Be(newRoom.Id);
        //    }

    }
}
