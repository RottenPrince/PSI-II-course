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
        private readonly IRoomRepository _roomRepo;
        private readonly IRepository<QuizModel> _quizRepo;
        private readonly IQuizQuestionRelationRepository _relationRepo;
        private readonly IDictionaryCache<int, RoomContentDTO> _roomCache;
        private readonly UserManager<ApplicationUser> _userManager;

        public LobbyAPIController(IMapper mapper, IRoomRepository roomRepo, IRepository<QuizModel> quizRepo, IQuizQuestionRelationRepository relationRepo,
            IDictionaryCache<int, RoomContentDTO> roomCache, UserManager<ApplicationUser> userManager)
        {
            _random = new Random();
            _mapper = mapper;
            _roomRepo = roomRepo;
            _quizRepo = quizRepo;
            _relationRepo = relationRepo;
            _roomCache = roomCache;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllRooms()
        {
            var userId = User.FindFirst("Id")?.Value;

            var user = await _userManager.Users
                .Include(u => u.Rooms)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var rooms = user.Rooms; //all rooms user has

            if (rooms == null)
                return NotFound();

            return Ok(_mapper.Map<List<RoomDTO>>(rooms));
        }

        [HttpGet("{roomId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllQuizzes(int roomId)
        {
            var userId = User.FindFirst("Id")?.Value;

            var user = await _userManager.Users
                .Include(u => u.Rooms)
                .ThenInclude(r => r.Quizs)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            var room = user.Rooms.FirstOrDefault(r => r.Id == roomId);

            if (room == null)
                return NotFound("Room not found");

            var quizzes = room.Quizs
                .Where(q => q.UserId == userId)
                .Select(q => new QuizDTO
                {
                    Id = q.Id,
                    StartTime = q.StartTime,
                })
                .ToList();

            return Ok(quizzes);
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
            string newCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var newRoom = new RoomModel { Name = roomName, UniqueCode = newCode };
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateQuiz(int roomId, int questionAmount)
        {
            var userId = User.FindFirst("Id")?.Value;
            var user = await _userManager.Users
                .Include(u => u.Quizzes)
                .FirstOrDefaultAsync(u => u.Id == userId);

            int quizId = await _relationRepo.CreateNewQuiz(roomId, questionAmount, userId, user); //added  userId, user to assign them on creation. not sure about that
            _roomCache.Invalidate(roomId);
            if(quizId == -1)
            {
                return NotFound();
            }
            else
            {
                var quiz = _quizRepo.GetById(quizId).Result;
                user.Quizzes.Add(quiz);

                return Ok(quizId);
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

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetRoomId(int quizId)
        {
            var quiz = await _quizRepo.GetById(quizId);
            if (quiz == null) return NotFound();

            return Ok(quiz.RoomId);
        }

        [HttpGet("{uniqueCode}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> JoinRoom(string uniqueCode)
        {
            var userId = User.FindFirst("Id")?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Find the room by unique code
            var room = _roomRepo.GetByUniqueCode(uniqueCode).Result;
            if (room == null)
            {
                return NotFound();
            }

            // Check if the user is already in the room
            if (user.Rooms != null && user.Rooms.Any(r => r.UniqueCode == uniqueCode))
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
