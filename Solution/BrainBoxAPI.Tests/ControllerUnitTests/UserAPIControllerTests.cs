using AutoMapper;
using BrainBoxAPI.Controllers;
using BrainBoxAPI.Data;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Lobby;
using SharedModels.Question;
using Xunit;
using FakeItEasy;
using BrainBoxAPI.Services;
using SharedModels.User;

namespace BrainBoxAPI.Tests.ControllerUnitTests
{
    public class UserAPIControllerTests
    {
        [Fact]
        public async Task GetUser_ReturnsUserDTO_WhenUserExists()
        {
            // Arrange
            var userManager = A.Fake<UserManager<ApplicationUser>>();
            var jwtService = A.Fake<JwtService>();

            var controller = new UserAPIController(userManager, jwtService);

            var existingUsername = "Vardenis";
            var existingUser = new ApplicationUser { UserName = existingUsername, Email = "vardenis.pavardenis@gmail.com" };

            A.CallTo(() => userManager.FindByNameAsync(existingUsername)).Returns(existingUser);

            // Act
            var result = await controller.GetUser(existingUsername);

            // Assert
            A.CallTo(() => userManager.FindByNameAsync(existingUsername)).MustHaveHappenedOnceExactly();
            var userDTO = Assert.IsType<ActionResult<UserDTO>>(result);
            Assert.NotNull(userDTO.Value);
            Assert.Equal(existingUser.UserName, userDTO.Value.UserName);
            Assert.Equal(existingUser.Email, userDTO.Value.Email);
        }
    }
}
