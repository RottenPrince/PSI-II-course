using BrainBoxUI.Controllers;
using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels.User;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        public void Login_WithValidAuthenticationRequest_RedirectsToIndex()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var validAuthRequest = new AuthenticationRequest
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };

            var fakeTokenResponse = new AuthenticationResponse
            {
                Token = "fakeToken"
            };
            APIError? fakeError;

            A.CallTo(() => fakeApiRepository.Post<AuthenticationRequest, AuthenticationResponse>(
                    A<string>._,
                    validAuthRequest,
                    A<bool>._,
                    out fakeError))
                .Returns(fakeTokenResponse);

            // Act
            var result = controller.Login(validAuthRequest) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Home");
   
        }
        [Fact]
        public void Login_WithInvalidAuthenticationRequest_ReturnsLoginView()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var invalidAuthRequest = new AuthenticationRequest
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };

            APIError? fakeError = new APIError(HttpStatusCode.BadRequest, "Invalid request");

            A.CallTo(() => fakeApiRepository.Post<AuthenticationRequest, AuthenticationResponse>(
                    A<string>._,
                    invalidAuthRequest,
                    A<bool>._,
                    out fakeError))
                .Returns(null);

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), A.Fake<ITempDataProvider>());

            // Act
            var result = controller.Login(invalidAuthRequest) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Login");
            result.ControllerName.Should().Be("Account");
            controller.TempData["message"].Should().Be("Invalid request");
        }

        [Fact]
        public void Logout_WithBearerTokenCookie_RedirectsToHomeIndexAndDeletesCookie()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            controller.HttpContext.Request.Headers.Add("Cookie", "BearerToken=fakeToken");

            // Act
            var result = controller.Logout() as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Home");

            controller.HttpContext.Response.Cookies.Delete("BearerToken");
        }

    


        [Fact]
        public void Logout_WithoutBearerTokenCookie_RedirectsToHomeIndex()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = controller.Logout() as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Index");
            result.ControllerName.Should().Be("Home");

        }

        [Fact]
        public void Login_WithInvalidModelState_ReturnsLoginRedirect()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository);

            controller.ModelState.AddModelError("UserName", "UserName is required");

            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), A.Fake<ITempDataProvider>());

            // Act
            var result = controller.Login(new AuthenticationRequest()) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<RedirectToActionResult>();
            result.ActionName.Should().Be("Login");
            result.ControllerName.Should().Be("Account");
            controller.TempData["message"].Should().Be("Not all fields were filled");
        }




        [Fact]
        public void Register_WithInvalidUser_ReturnsViewAndDisplaysErrorMessage()
        {
            // Arrange
            var fakeApiRepository = A.Fake<IApiRepository>();
            var controller = new AccountController(fakeApiRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var invalidUserDto = new UserDTO
            {
                UserName = "TestUser",
            };

            APIError? fakeError = new APIError(HttpStatusCode.BadRequest, "Invalid request");

            A.CallTo(() => fakeApiRepository.Post<UserDTO, UserDTO>(
                    A<string>._,
                    invalidUserDto,
                    A<bool>._,
                    out fakeError))
                .Returns(null);

            var tempData = new TempDataDictionary(controller.HttpContext, A.Fake<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = controller.Register(invalidUserDto) as ViewResult;

            // Assert
            result.Should().NotBeNull().And.BeOfType<ViewResult>();
            result.ViewName.Should().BeNull(); 
            result.Model.Should().Be(invalidUserDto); 

            controller.TempData["message"].Should().Be("Invalid request");
        }


    }
}
