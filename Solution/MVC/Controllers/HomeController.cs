using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using MVC.Models;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Lobby/{room}")]
        public IActionResult Lobby(string room)
        {
            var roomName = APIHelper.Get<string>($"LobbyAPI/GetRoomName/{room}", out APIError? error);

            if(error == null)
            {
                ViewBag.RoomName = roomName;
                ViewBag.Room = room;
            }
            else
            {
                ViewBag.ErrorMessage = error.Message;
            }
            

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
