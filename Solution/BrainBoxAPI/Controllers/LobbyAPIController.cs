using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxAPI.Managers;
using SharedModels.Lobby;
using BrainBoxAPI.Models;
using AutoMapper;
using BrainBoxAPI.Exceptions;
using BrainBoxAPI.Caching;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BrainBoxAPI.Data;

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
        private readonly UserManager<ApplicationUser> _userManager;

        public LobbyAPIController(IMapper mapper, IRepository<RoomModel> roomRepo, IQuizQuestionRelationRepository relationRepo,
            IDictionaryCache<int, RoomContentDTO> roomCache, UserManager<ApplicationUser> userManager)
        {
            _random = new Random();
            _mapper = mapper;
            _roomRepo = roomRepo;
            _relationRepo = relationRepo;
            _roomCache = roomCache;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllRooms()
        {
            var userId = User.FindFirst("Id")?.Value;
            Console.Write("USER ID2: " + userId);

            var user = await _userManager.Users
                .Include(u => u.Rooms)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var rooms = user.Rooms; //all rooms user has

            if (rooms == null)
                return NotFound();

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
        public async Task<IActionResult> GetRoomId(int runId) //doesn't work with empty quiz, because searches by QuizRelations
        {
            var questions = await _relationRepo.GetAllQuizQuestionsInfo(runId);
            if (questions != null && questions.Any())
            {
                int roomId = questions.First().Question.RoomId; 
                return Ok(roomId);
            }

            return NotFound();
        }

        [HttpPost("{roomId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> JoinRoom(int roomId)  //roomId + user Token, connects room and user
        {
            var userId = User.FindFirst("Id")?.Value;

            Console.Write("USER ID: " + userId);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }


            var room = _roomRepo.GetById(roomId).Result;
            if (room == null)
            {
                return NotFound();
            }

            // Check if the user is already in the room
            if (user.Rooms != null && user.Rooms.Any(r => r.Id == roomId))
            {
                return BadRequest("User is already in the room.");
            }

            // Add the user to the room
            user.Rooms.Add(room);
            await _userManager.UpdateAsync(user);

            return Ok("User joined the room successfully.");
        }
    }


}
