using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Lobby;
using SharedModels.User;

namespace BrainBoxUI.Controllers
{
    public class AccountController : Controller
    {

        private readonly IApiRepository _apiRepository;

        public AccountController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View(new AuthenticationRequest());
        }

        [HttpPost]
        public IActionResult Login(AuthenticationRequest authRequest)
        {
            if (ModelState.IsValid)
            {
                var tokenResponse = _apiRepository.Post<AuthenticationRequest, AuthenticationResponse>($"api/User/CreateBearerToken", authRequest, includeBearerToken: false, out APIError? error2);

                if(tokenResponse != null)
                {
                    HttpContext.Response.Cookies.Append("BearerToken", tokenResponse.Token, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1)
                    });
                }

                return RedirectToAction("Index", "Home");
            }

            return View(authRequest);
        }

        public IActionResult Register()
        {
            return View(new UserDTO());
        }

        [HttpPost]
        public IActionResult Register(UserDTO userDto)
        {
            if (ModelState.IsValid)
            {
                _apiRepository.Post<UserDTO, UserDTO>($"api/User/PostUser", userDto, includeBearerToken: false, out APIError? error1);
                var authRequest = new AuthenticationRequest
                {
                    UserName = userDto.UserName,
                    Password = userDto.Password,
                };
                var createResponse = _apiRepository.Post<AuthenticationRequest,AuthenticationResponse>($"api/User/PostUser", authRequest, includeBearerToken: false, out APIError? error2);

                return RedirectToAction("Login");
            }
            return View(userDto);
        }

        public IActionResult Logout()
        {
            if (!HttpContext.Request.Cookies.FirstOrDefault(pr => pr.Key == "BearerToken").Equals(default(KeyValuePair<String, String>)))
                HttpContext.Response.Cookies.Delete("BearerToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
