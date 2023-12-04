using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxAPI.Managers;
using SharedModels.Lobby;
using BrainBoxAPI.Models;
using AutoMapper;
using BrainBoxAPI.Exceptions;
using BrainBoxAPI.Caching;
using Microsoft.AspNetCore.Authorization;

namespace BrainBoxAPI.Controllers
{
    [Route("api/Lobby/[action]")]
    public class LobbyAPIController : Controller
    {
        private Random _random;
        private readonly IMapper _mapper;
        private readonly IRepository<RoomModel> _roomRepo;
        private readonly IQuizQuestionRelationRepository _relationRepo;
        private readonly IDictionaryCache<int, RoomContentDTO> _roomCache;

        public LobbyAPIController(IMapper mapper, IRepository<RoomModel> roomRepo, IQuizQuestionRelationRepository relationRepo, IDictionaryCache<int, RoomContentDTO> roomCache)
        {
            _random = new Random();
            _mapper = mapper;
            _roomRepo = roomRepo;
            _relationRepo = relationRepo;
            _roomCache = roomCache;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepo.GetAll();
            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            var model = _roomCache.GetOrCompute(roomId, id =>
            {
                RoomModel? room = _roomRepo.GetById(id).Result;
                if (room == null) return null;
                return new RoomContentDTO
                {
                    RoomName = room.Name,
                    QuestionAmount = room.Questions.Count,
                };
            });
            return Ok(model);
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
            _roomCache.Invalidate(newRoom.Id);
            return Ok(newRoom.Id);
        }

        [HttpGet("{roomId}/{questionAmount}")]
        public async Task<IActionResult> CreateQuiz(int roomId, int questionAmount)
        {
            int id = await _relationRepo.CreateNewQuiz(roomId, questionAmount);
            _roomCache.Invalidate(roomId);
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
