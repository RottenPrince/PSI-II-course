using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxAPI.Managers;
using AutoMapper;
using BrainBoxAPI.Models;
using BrainBoxAPI.Caching;
using SharedModels.Lobby;

namespace BrainBoxAPI.Controllers
{
    [Route("api/Question/[action]")]
    [ApiController]
    public class QuestionAPIController : ControllerBase
    {
        private readonly ILogger<QuestionAPIController> _logger;
        private Random _random;
        private readonly IMapper _mapper;
        private readonly IRepository<QuestionModel> _questionRepo;
        private readonly IDictionaryCache<int, RoomContentDTO> _roomCache;

        public QuestionAPIController(ILogger<QuestionAPIController> logger, IMapper mapper, IRepository<QuestionModel> questionRepo, IDictionaryCache<int, RoomContentDTO> roomCache)
        {
            _logger = logger;
            _mapper = mapper;
            _questionRepo = questionRepo;
            _random = new Random();
            _roomCache = roomCache;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var questionModel = await _questionRepo.GetById(questionId);
            if (questionModel == null) return NotFound();
            return Ok(_mapper.Map<QuestionDTO>(questionModel));
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetFullQuestion(int questionId)
        {
            var questionModel = await _questionRepo.GetById(questionId);
            if (questionModel == null) return NotFound();
            return Ok(_mapper.Map<QuestionWithAnswerDTO>(questionModel));
        }

        [HttpPost("{roomId}")]
        public async Task<IActionResult> SaveQuestion(int roomId, [FromBody] QuestionWithAnswerDTO questionModel)
        {
            if (questionModel == null)
            {
                return BadRequest("Question data is invalid");
            }

            var dbModel = _mapper.Map<QuestionModel>(questionModel);

            dbModel.RoomId = roomId;
            _questionRepo.Add(dbModel);
            _questionRepo.Save();
            _roomCache.Invalidate(roomId);
            return Ok("Question created successfully.");
        }

        [HttpPost("{roomId}")]
        public async Task<IActionResult> SaveMultipleQuestions(int roomId, [FromBody] List<QuestionWithAnswerDTO> questionModels)
        {
            if (questionModels == null)
            {
                return BadRequest("Question data is invalid");
            }

            var dbModels = _mapper.Map<List<QuestionModel>>(questionModels);
            foreach(var model in dbModels)
            {
                model.RoomId = roomId;
                _questionRepo.Add(model);
            }
            _questionRepo.Save();
            _roomCache.Invalidate(roomId);
            return Ok("Questions created successfully.");
        }
    }
}
