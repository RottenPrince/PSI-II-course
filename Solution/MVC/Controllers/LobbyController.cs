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
            var roomContent = APIHelper.Get<RoomContentStruct>($"LobbyAPI/GetRoomContent/{roomId}", out APIError? error);

            if (error == null)
            {
                ViewBag.RoomName = roomContent.RoomName;
                ViewBag.QuestionAmount = roomContent.QuestionAmount;
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
            var rooms = APIHelper.Get<List<RoomTransferModel>>("LobbyAPI/GetAllRooms", out _);
            return View(rooms);
        }

        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult CreateRoom(string roomName)
        {
            APIHelper.Post<string, string>("LobbyAPI/CreateRoom", roomName, out var error);
            ViewBag.RoomName = roomName;
            if(error != null)
            {
                ViewBag.ErrorMessage = error.Message;
                return View("CreateError");
            }
            return View("CreateSuccess");
        }
    }
}
