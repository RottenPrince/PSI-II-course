using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using SharedModels.Lobby;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyAPIController : Controller
    {
        [HttpGet ("{roomId}")]
        public IActionResult GetRoomName(int roomId)
        {
            var roomName = QuestionManager.GetRoomName(roomId);
            return Ok(roomName);
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            return Ok(QuestionManager.GetAllRooms());
        }

        [HttpGet("{roomId}")]
        public IActionResult GetAllQuestions(int roomId)
        {
            return Ok(QuestionManager.GetAllQuestions(roomId));
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRoomContent(int roomId)
        {
            var roomModel = QuestionManager.GetRoomContent(roomId);
            return Ok(roomModel);
        }
    }
}
