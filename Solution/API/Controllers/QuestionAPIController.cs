using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using AutoMapper;
using API.Data;

namespace API.Controllers
{
    [Route("api/Question/[action]")]
    [ApiController]
    public class QuestionAPIController : ControllerBase
    {
        private readonly ILogger<QuestionAPIController> _logger;
        private Random _random;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public QuestionAPIController(ILogger<QuestionAPIController> logger, IMapper mapper, AppDbContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var questionModel = await QuestionManager.GetQuestionWithAnswer(_context, questionId);
            if (questionModel == null) return NotFound();
            return Ok(_mapper.Map<QuestionTransferModel>(questionModel));
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetFullQuestion(int questionId)
        {
            var questionModel = await QuestionManager.GetQuestionWithAnswer(_context, questionId);
            if (questionModel == null) return NotFound();
            return Ok(_mapper.Map<QuestionWithAnswerTransferModel>(questionModel));
        }

        [HttpPost("{roomId}")]
        public IActionResult SaveQuestion(int roomId, [FromBody] QuestionWithAnswerTransferModel questionModel)
        {
            if (questionModel == null)
            {
                return BadRequest("Question data is invalid");
            }

            QuestionManager.CreateNewQuestion(_context, roomId, questionModel);
            return Ok("Question created successfully.");
        }
    }
}
