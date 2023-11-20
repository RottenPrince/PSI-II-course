using Microsoft.AspNetCore.Mvc;
using BrainBoxUI.Helpers.API;
using SharedModels.Question;
using SharedModels.Lobby;

namespace BrainBoxUI.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public LobbyController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        [HttpGet("{roomId}")]
        public IActionResult Room(string roomId)
        {
            var roomContent = _apiRepository.Get<RoomContentDTO>($"api/Lobby/GetRoomContent/{roomId}", out APIError? error);

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
            var rooms = _apiRepository.Get<List<RoomDTO>>("api/Lobby/GetAllRooms", out _);
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
            ViewBag.RoomId = _apiRepository.Post<string, int>("api/Lobby/CreateRoom", roomName, out var error);
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
