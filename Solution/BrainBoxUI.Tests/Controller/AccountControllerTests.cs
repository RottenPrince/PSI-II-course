using System;
using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedModels.User;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Xunit;
using FakeItEasy;

namespace BrainBoxUI.Tests.Controller
{
    public class AccountControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Login_ReturnsViewResultWithAuthenticationRequestModel()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository);

            // Act
            var result = controller.Login() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsType<AuthenticationRequest>(result.Model);
        }

        [Fact]
        public void Register_ReturnsViewResultWithUserDTOModel()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository);

            // Act
            var result = controller.Register() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsType<UserDTO>(result.Model);
            Assert.Null(result.ViewName);
            Assert.True(result.ViewData.ModelState.IsValid); 
        }

        [Fact]
                public void Register_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>(); 
            var controller = new AccountController(fakeApiRepository);

            var validUserDto = new UserDTO
            {
                UserName = "TestUser",
                Password = "TestPassword",
                Email = "test@example.com"
            };

            // Act
            var result = controller.Register(validUserDto) as RedirectToActionResult;
            APIError? fakeError;
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.True(controller.ModelState.IsValid);
            A.CallTo(() => fakeApiRepository.Post<UserDTO, UserDTO>(A<string>.Ignored, validUserDto, false, out fakeError))
    .MustHaveHappenedOnceExactly();

            A.CallTo(() => fakeApiRepository.Post<AuthenticationRequest, AuthenticationResponse>(A<string>.Ignored, A<AuthenticationRequest>.That.Matches(req => req.UserName == validUserDto.UserName && req.Password == validUserDto.Password), false, out fakeError))
                .MustHaveHappenedOnceExactly();

        }
 
    }
}
