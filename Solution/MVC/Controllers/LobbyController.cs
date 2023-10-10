using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using SharedModels.Question;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyController : Controller
    {
        [HttpGet("{roomId}")]
        public IActionResult Room(string roomId)
        {
            var roomName = APIHelper.Get<string>($"LobbyAPI/GetRoomName/{roomId}", out APIError? error);

            if (error == null)
            {
                ViewBag.RoomName = roomName;
                ViewBag.Room = roomId;
            }
            else
            {
                ViewBag.ErrorMessage = error.Message;
            }


            return View();
        }

        [HttpGet]
        public IActionResult AllRooms()
        {
            var rooms = APIHelper.Get<List<RoomModel>>("LobbyAPI/GetAllRooms", out _);
            return View(rooms);
        }
    }
}
