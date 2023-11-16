using Microsoft.AspNetCore.Mvc;
using BrainBoxUI.Helpers.BrainBoxAPI;
using SharedModels.Question;
using SharedModels.Lobby;

namespace BrainBoxUI.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyController : Controller
    {
        [HttpGet("{roomId}")]
        public IActionResult Room(string roomId)
        {
            var roomContent = APIHelper.Get<RoomContentStruct>($"api/Lobby/GetRoomContent/{roomId}", out APIError? error);

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
            var rooms = APIHelper.Get<List<RoomTransferModel>>("api/Lobby/GetAllRooms", out _);
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
            ViewBag.RoomId = APIHelper.Post<string, int>("api/Lobby/CreateRoom", roomName, out var error);
            if (error != null)
            {
                ViewBag.ErrorMessage = error.Message;
                return View("CreateError");
            }
            ViewBag.RoomName = roomName;

            return View("CreateSuccess");
        }
    }
}
