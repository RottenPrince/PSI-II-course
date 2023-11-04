using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using SharedModels.Lobby;
using API.Data;
using API.Models;

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
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            var roomModel = await QuestionManager.GetRoomContent(roomId);
            return Ok(roomModel);
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] string roomName)
        {
            using(var db = new AppDbContext())
            {
                var newModel = new RoomModel { Name = roomName };
                if(db.Rooms.Where(r => r.Name == roomName).Count() != 0)
                {
                    return BadRequest("Name already taken");
                }

                db.Rooms.Add(new RoomModel { Name = roomName });
                db.SaveChanges();
                return Ok("Room successfully added");
            }
        }
    }
}
