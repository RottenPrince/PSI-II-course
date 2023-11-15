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
        private readonly IRepository<RoomModel> _rooms;
        private readonly IQuestionSolveRunJoinRepository _runs;

        public LobbyAPIController(IMapper mapper, IRepository<RoomModel> rooms, IQuestionSolveRunJoinRepository runs)
        {
            _random = new Random();
            _mapper = mapper;
            _rooms = rooms;
            _runs = runs;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _rooms.GetAll();
            return Ok(_mapper.Map<List<RoomTransferModel>>(rooms));
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            RoomModel? room = await _rooms.GetById(roomId);
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
            _rooms.Add(new RoomModel { Name = roomName });
            try
            {
                _rooms.Save();
            } catch (DbUpdateException ex) {
                var message = ex.InnerException.Message;
                var errorCode = Regex.Match(message, "^.*Error (\\d+):.*$");
                if (!errorCode.Success || errorCode.Groups.Count < 2) { throw ex; }
                var errorCodeInt = int.Parse(errorCode.Groups[1].Value);
                if (errorCodeInt == 19) { return BadRequest("Name already in use"); }
                else { throw ex; }
            }
            return Ok("Room successfully added");
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> CreateSolveRun(int roomId)
        {
            int id = await _runs.CreateNewSolveRun(roomId);
            if(id == -1)
            {
                return NotFound();
            }
            else
            {
                return Ok(id);
            }
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetNextQuestionInRun(int runId)
        {
            var model = await _runs.GetNextQuestionInRun(runId);
            if (model == null) return NoContent();
            return Ok(_mapper.Map<QuestionTransferModel>(model.Question));
        }

        [HttpPost("{runId}/{answerId}")]
        public async Task<IActionResult> SubmitAnswer(int runId, int answerId)
        {
            var model = await _runs.GetNextQuestionInRun(runId);
            if (model == null) return BadRequest();
            model.SelectedAnswerOption = model.Question.AnswerOptions[answerId];
            _runs.Save();
            return Ok();
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetAllQuestionRunInfo(int runId)
        {
            var questions = await _runs.GetAllQuestionRunInfo(runId);
            if (questions.Any(x => x.SelectedAnswerOption == null)) return Unauthorized();
            return Ok(_mapper.Map<List<QuestionRunTransferModel>>(questions));
        }
    }
}
