using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using SharedModels.Lobby;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using AutoMapper;

namespace API.Controllers
{
    [Route("api/Lobby/[action]")]
    public class LobbyAPIController : Controller
    {
        private Random _random;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public LobbyAPIController(IMapper mapper, AppDbContext context)
        {
            _random = new Random();
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await QuestionManager.GetAllRooms(_context);
            return Ok(rooms.Select(r => _mapper.Map<RoomTransferModel>(r)));
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            RoomModel? room = await QuestionManager.GetRoomContent(_context, roomId);
            if (room == null) return NotFound();
            return Ok(new RoomContentStruct
            {
                RoomName = room.Name,
                QuestionAmount = room.Questions.Count,
            });
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] string roomName)
        {
            using (var db = new AppDbContext())
            {
                var newRoom = new RoomModel { Name = roomName };
                db.Rooms.Add(newRoom);
                try
                {
                    db.SaveChanges();
                } catch (DbUpdateException ex) {
                    var message = ex.InnerException.Message;
                    var errorCode = Regex.Match(message, "^.*Error (\\d+):.*$");
                    if (!errorCode.Success || errorCode.Groups.Count < 2) { throw ex; }
                    var errorCodeInt = int.Parse(errorCode.Groups[1].Value);
                    if (errorCodeInt == 19) { return BadRequest("Name already in use"); }
                    else { throw ex; }
                }
                return Ok(newRoom.Id);
            }
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> CreateSolveRun(int roomId)
        {
            try
            {
                return Ok(await QuestionManager.CreateNewSolveRun(_context, _random, roomId));
            } catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetNextQuestionInRun(int runId)
        {
            var model = await QuestionManager.GetNextQuestionInRun(_context, runId);
            if (model == null) return NoContent();
            return Ok(_mapper.Map<QuestionTransferModel>(model.Question));
        }

        [HttpPost("{runId}/{answerId}")]
        public async Task<IActionResult> SubmitAnswer(int runId, int answerId)
        {
            var model = await QuestionManager.GetNextQuestionInRun(_context, runId);
            if (model == null) return BadRequest();
            model.SelectedAnswerOption = model.Question.AnswerOptions[answerId];
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetAllQuestionRunInfo(int runId)
        {
            var questions = await QuestionManager.GetAllQuestionRunInfo(_context, runId);
            if (questions.Any(x => x.SelectedAnswerOption == null)) return Unauthorized();
            return Ok(_mapper.Map<List<QuestionRunTransferModel>>(questions));
        }
    }
}
