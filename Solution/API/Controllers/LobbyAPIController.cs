using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using SharedModels.Lobby;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [Route("api/Lobby/[action]")]
    public class LobbyAPIController : Controller
    {
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            return Ok(QuestionManager.GetAllRooms());
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
                db.Rooms.Add(new RoomModel { Name = roomName });
                try
                {
                    db.SaveChanges();
                } catch (DbUpdateException ex) {
                    var message = ex.InnerException.Message;
                    var errorCode = Regex.Match(message, "^.*Error (\\d+):.*$");
                    if(!errorCode.Success || errorCode.Groups.Count < 2) { throw ex; }
                    var errorCodeInt = int.Parse(errorCode.Groups[1].Value);
                    if(errorCodeInt == 19) { return BadRequest("Name already in use"); }
                    else { throw ex; }
                }
                return Ok("Room successfully added");
            }
        }
    }
}
