using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxAPI.Managers;
using SharedModels.Lobby;
using BrainBoxAPI.Models;
using AutoMapper;
using BrainBoxAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BrainBoxAPI.Data;
using BrainBoxAPI.Extensions;

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
        private readonly UserManager<ApplicationUser> _userManager;

        public LobbyAPIController(IMapper mapper, IRoomRepository roomRepo, IRepository<QuizModel> quizRepo, IQuizQuestionRelationRepository relationRepo,
            UserManager<ApplicationUser> userManager)
        {
            _random = new Random();
            _mapper = mapper;
            _roomRepo = roomRepo;
            _quizRepo = quizRepo;
            _relationRepo = relationRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllRooms()
        {
            var user = await _userManager.FromClaim(User);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetRoomContent(int roomId)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Rooms.Any(r => r.Id == roomId))
            {
                return Unauthorized();
            }

            RoomModel? room = _roomRepo.GetById(roomId).Result;
            if (room == null) return NotFound();
            var model = new RoomContentDTO
            {
                RoomName = room.Name,
                QuestionAmount = room.Questions.Count,
                UniqueCode = room.UniqueCode,
            };
            return Ok(model);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateRoom([FromBody] string roomName)
        {
            var user = await _userManager.FromClaim(User);
            string newCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var newRoom = new RoomModel { Name = roomName, UniqueCode = newCode };
            _roomRepo.Add(newRoom);
            user.Rooms.Add(newRoom);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateQuiz(int roomId, int questionAmount)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Rooms.Any(r => r.Id == roomId))
            {
                return Unauthorized();
            }

            int quizId = await _relationRepo.CreateNewQuiz(roomId, questionAmount, user.Id, user); //added  userId, user to assign them on creation. not sure about that
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetNextQuestionInQuiz(int runId)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Quizzes.Any(q => q.Id == runId))
            {
                return Unauthorized();
            }
            var model = await _relationRepo.GetNextQuestionInQuiz(runId);
            if (model == null) return NoContent();
            return Ok(_mapper.Map<QuestionDTO>(model.Question));
        }

        [HttpPost("{runId}/{answerId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> SubmitAnswer(int runId, int answerId)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Quizzes.Any(q => q.Id == runId))
            {
                return Unauthorized();
            }
            var model = await _relationRepo.GetNextQuestionInQuiz(runId);
            if (model == null) return BadRequest();
            model.SelectedAnswerOption = model.Question.AnswerOptions[answerId];
            _relationRepo.Save();
            return Ok();
        }

        [HttpGet("{runId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAllQuizQuestionsInfo(int runId)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Quizzes.Any(q => q.Id == runId))
            {
                return Unauthorized();
            }
            var questions = await _relationRepo.GetAllQuizQuestionsInfo(runId);
            if (questions.Any(x => x.SelectedAnswerOption == null)) return Unauthorized();
            return Ok(_mapper.Map<List<QuizQuestionDTO>>(questions));
        }

        [HttpGet("{runId}/{currentQuestionIndex}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetNextQuestionInReview(int runId, int currentQuestionIndex)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Quizzes.Any(q => q.Id == runId))
            {
                return Unauthorized();
            }
            var model = await _relationRepo.GetNextQuestionInReview(runId, currentQuestionIndex);
            if(model == null)
                return NoContent();
            return Ok(_mapper.Map<QuizQuestionDTO>(model));
        }

        [HttpGet("{quizId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetRoomId(int quizId)
        {
            var user = await _userManager.FromClaim(User);
            if(!user.Quizzes.Any(q => q.Id == quizId))
            {
                return Unauthorized();
            }
            var quiz = await _quizRepo.GetById(quizId);
            if (quiz == null) return NotFound();

            return Ok(quiz.RoomId);
        }

        [HttpGet("{uniqueCode}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> JoinRoom(string uniqueCode)
        {
            var user = await _userManager.FromClaim(User);

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
