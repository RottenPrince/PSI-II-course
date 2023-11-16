using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxAPI.Managers;
using SharedModels.Lobby;
using BrainBoxAPI.Data;
using BrainBoxAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System;
using AutoMapper;
using BrainBoxAPI.Exceptions;

namespace BrainBoxAPI.Controllers
{
    [Route("api/Lobby/[action]")]
    public class LobbyAPIController : Controller
    {
        private Random _random;
        private readonly IMapper _mapper;
        private readonly IRepository<RoomModel> _roomRepo;
        private readonly IQuizQuestionRelationRepository _relationRepo;

        public LobbyAPIController(IMapper mapper, IRepository<RoomModel> roomRepo, IQuizQuestionRelationRepository relationRepo)
        {
            _random = new Random();
            _mapper = mapper;
            _roomRepo = roomRepo;
            _relationRepo = relationRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepo.GetAll();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            RoomModel? room = await _roomRepo.GetById(roomId);
            if (room == null) return NotFound();
            return Ok(new RoomContentDTO
            {
                RoomName = room.Name,
                QuestionAmount = room.Questions.Count,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] string roomName)
        {
            var newRoom = new RoomModel { Name = roomName };
            _roomRepo.Add(newRoom);
            try
            {
                _roomRepo.Save();
            }
            catch (DbConstraintFailedException)
            {
                return BadRequest("Name already in use");
            }
            return Ok(newRoom.Id);
        }

        [HttpGet("{roomId}/{questionAmount}")]
        public async Task<IActionResult> CreateQuiz(int roomId, int questionAmount)
        {
            int id = await _relationRepo.CreateNewQuiz(roomId, questionAmount);
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
        public async Task<IActionResult> GetNextQuestionInQuiz(int runId)
        {
            var model = await _relationRepo.GetNextQuestionInQuiz(runId);
            if (model == null) return NoContent();
            return Ok(_mapper.Map<QuestionDTO>(model.Question));
        }

        [HttpPost("{runId}/{answerId}")]
        public async Task<IActionResult> SubmitAnswer(int runId, int answerId)
        {
            var model = await _relationRepo.GetNextQuestionInQuiz(runId);
            if (model == null) return BadRequest();
            model.SelectedAnswerOption = model.Question.AnswerOptions[answerId];
            _relationRepo.Save();
            return Ok();
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetAllQuizQuestionsInfo(int runId)
        {
            var questions = await _relationRepo.GetAllQuizQuestionsInfo(runId);
            if (questions.Any(x => x.SelectedAnswerOption == null)) return Unauthorized();
            return Ok(_mapper.Map<List<QuizQuestionDTO>>(questions));
        }

        [HttpGet("{runId}/{currentQuestionIndex}")]
        public async Task<IActionResult> GetNextQuestionInReview(int runId, int currentQuestionIndex)
        {
            var model = await _relationRepo.GetNextQuestionInReview(runId, currentQuestionIndex);
            if(model == null)
                return NoContent();
            return Ok(_mapper.Map<QuizQuestionDTO>(model));
        }

        [HttpGet("{runId}")]
        public async Task<IActionResult> GetRoomId(int runId)
        {
            var questions = await _relationRepo.GetAllQuizQuestionsInfo(runId);
            if (questions != null && questions.Any())
            {
                int roomId = questions.First().Question.RoomId; 
                return Ok(roomId);
            }

            return NotFound();
        }
    }
}
