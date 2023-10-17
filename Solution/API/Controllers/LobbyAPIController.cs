using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using SharedModels.Lobby;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyAPIController : Controller
    {
        [HttpGet ("{room}")]
        public IActionResult GetRoomName(string room)
        {
            var roomName = QuestionManager.GetRoomName(room, out var error);
            if (error != null)
                return error;

            return Ok(roomName);
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            return Ok(QuestionManager.GetAllRooms());
        }

        [HttpGet("{room}")]
        public IActionResult GetAllQuestions(string room)
        {
            return Ok(QuestionManager.GetAllQuestions(room));
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRoomContent(string roomId)
        {
            var roomModel = QuestionManager.GetRoomContent(roomId, out var error);
            if (error != null)
                return error;

            return Ok(roomModel);
        }
    }
}
