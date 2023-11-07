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
        private IMapper _mapper;

        public LobbyAPIController(IMapper mapper)
        {
            _random = new Random();
            _mapper = mapper;
        }

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

        [HttpGet("{roomId}")]
        public IActionResult CreateSolveRun(int roomId)
        {
            using(var db = new AppDbContext())
            {
                var roomModel = db.Rooms
                                .Include(room => room.Questions)
                                .Where(room => room.Id == roomId)
                                .First();
                var questions = roomModel.Questions;
                lock(_random)
                {
                    questions = questions.OrderBy(x => _random.Next()).ToList();
                }
                db.Rooms.Attach(roomModel);
                db.Questions.AttachRange(questions);
                var newModel = new SolveRunModel
                {
                    StartTime = DateTime.UtcNow,
                    Room = roomModel,
                    QuestionRun = questions,
                };
                db.SolveRunModels.Add(newModel);
                db.SaveChanges();
                return Ok(newModel.Id);
            }
        }

        private QuestionSolveRunJoinModel GetNextQuestionFromDB(AppDbContext db, int runId)
        {
            var runModel = db.SolveRunModels
                .Include(srm => srm.SolveRunJoin)
                .ThenInclude(a => a.Question)
                .ThenInclude(q => q.AnswerOptions)
                .Where(srm => srm.Id == runId).FirstOrDefault();
            var questions = runModel.SolveRunJoin;
            try
            {
                return questions.First(m => m.SelectedAnswerOption == null);
            } catch (InvalidOperationException)
            {
                return null;
            }
        }

        [HttpGet("{runId}")]
        public IActionResult GetNextQuestionInRun(int runId)
        {
            using(var db = new AppDbContext())
            {
                var model = GetNextQuestionFromDB(db, runId);
                if (model == null) return NoContent();
                return Ok(_mapper.Map<QuestionTransferModel>(model.Question));
            }
        }

        [HttpPost("{runId}")]
        public IActionResult SubmitAnswer(int runId, int answerId)
        {
            using(var db = new AppDbContext())
            {
                var model = GetNextQuestionFromDB(db, runId);
                if (model == null) return BadRequest();
                model.SelectedAnswerOption = model.Question.AnswerOptions[answerId];
                db.SaveChanges();
                return Ok();
            }
        }
    }
}
