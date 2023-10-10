using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using SharedModels.Question;
using SharedModels.Lobby;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyController : Controller
    {
        [HttpGet("{roomId}")]
        public IActionResult Room(string roomId)
        {
            var roomModel = APIHelper.Get<RoomContentModel>($"LobbyAPI/GetRoomContent/{roomId}", out APIError? error);

            if (error == null)
            {
                ViewBag.RoomName = roomModel.RoomName;
                ViewBag.QuestionAmount = roomModel.QuestionAmount;
                ViewBag.RoomId = roomId;
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
