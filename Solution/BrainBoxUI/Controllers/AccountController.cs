using BrainBoxUI.Helpers.API;
using Microsoft.AspNetCore.Mvc;
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
            if (!ModelState.IsValid)
            {
                TempData["message"] = "Not all fields were filled";
                return RedirectToAction("Login", "Account");
            }

            var tokenResponse = _apiRepository.Post<AuthenticationRequest, AuthenticationResponse>($"api/User/CreateBearerToken", authRequest, includeBearerToken: false, out APIError? error2);

            if(tokenResponse != null)
            {
                HttpContext.Response.Cookies.Append("BearerToken", tokenResponse.Token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1)
                });
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["message"] = error2.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Register()
        {
            return View(new UserDTO());
        }

        [HttpPost]
        public IActionResult Register(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = "Not all fields were filled";
                return View(userDto);
            }

            _apiRepository.Post<UserDTO, UserDTO>($"api/User/PostUser", userDto, includeBearerToken: false, out APIError? error1);
            if(error1 != null)
            {
                TempData["message"] = error1.Message;
                return View(userDto);
            }

            TempData["message"] = "Account created successfully";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            if (!HttpContext.Request.Cookies.FirstOrDefault(pr => pr.Key == "BearerToken").Equals(default(KeyValuePair<String, String>)))
                HttpContext.Response.Cookies.Delete("BearerToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
