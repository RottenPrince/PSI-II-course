using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;
using AutoMapper;
using API.Data;
using API.Models;

namespace API.Controllers
{
    [Route("api/Question/[action]")]
    [ApiController]
    public class QuestionAPIController : ControllerBase
    {
        private readonly ILogger<QuestionAPIController> _logger;
        private Random _random;
        private readonly IMapper _mapper;
        private readonly IRepository<QuestionModel> _questions;

        public QuestionAPIController(ILogger<QuestionAPIController> logger, IMapper mapper, IRepository<QuestionModel> questions)
        {
            _logger = logger;
            _mapper = mapper;
            _questions = questions;
            _random = new Random();
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var questionModel = await _questions.GetById(questionId);
            if (questionModel == null) return NotFound();
            return Ok(_mapper.Map<QuestionTransferModel>(questionModel));
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetFullQuestion(int questionId)
        {
            var questionModel = await _questions.GetById(questionId);
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

            var dbModel = _mapper.Map<QuestionModel>(questionModel);
            _questions.Add(dbModel);
            _questions.Save();
            return Ok("Question created successfully.");
        }
    }
}
