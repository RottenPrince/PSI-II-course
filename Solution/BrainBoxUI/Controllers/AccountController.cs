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

                if(HttpContext.Session != null)
                    HttpContext.Session.SetString("UserToken", tokenResponse.Token);


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
            if (HttpContext.Session != null && (HttpContext.Session.GetString("UserToken") != null))
                HttpContext.Session.Remove("UserToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
