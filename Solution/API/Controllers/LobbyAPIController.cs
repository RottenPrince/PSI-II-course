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
using API.Exceptions;

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
        public async Task<IActionResult> CreateRoom([FromBody] string roomName)
        {
            var newRoom = new RoomModel { Name = roomName };
            _rooms.Add(newRoom);
            try
            {
                _rooms.Save();
            }
            catch (DbConstraintFailedException)
            {
                return BadRequest("Name already in use");
            }
            return Ok(newRoom.Id);
        }

        [HttpGet("{roomId}/{questionAmount}")]
        public async Task<IActionResult> CreateSolveRun(int roomId, int questionAmount)
        {
            int id = await _runs.CreateNewSolveRun(roomId, questionAmount);
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

        [HttpGet("{runId}/{currentQuestionIndex}")]
        public async Task<IActionResult> GetNextQuestionInReview(int runId, int currentQuestionIndex)
        {
            var model = await _runs.GetNextQuestionInReview(runId, currentQuestionIndex);
            if(model == null)
                return NoContent();
            return Ok(_mapper.Map<QuestionRunTransferModel>(model));
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetRoomId(int runId)
        {
            var questions = await _runs.GetAllQuestionRunInfo(runId);
            if (questions != null && questions.Any())
            {
                int roomId = questions.First().Question.RoomId; 
                return Ok(roomId);
            }

            return NotFound();
        }
    }
}
