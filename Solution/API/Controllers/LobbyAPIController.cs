using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Databases;
using SharedModels.Lobby;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyAPIController : Controller
    {
        [HttpGet ("{room}")]
        public IActionResult GetRoomName(string room)
        {
            var roomName = QuestionDatabase.GetRoomName(room, out var error);
            if (error != null)
                return error;

            return Ok(roomName);
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            return Ok(QuestionDatabase.GetAllRooms());

        [HttpGet("{roomId}")]
        public IActionResult GetRoomContent(string roomId)
        {
            var roomModel = QuestionDatabase.GetRoomContent(roomId, out var error);
            if (error != null)
                return error;

            return Ok(roomModel);
        }
    }
}
