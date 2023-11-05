using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;

namespace API.Controllers
{
    [Route("api/Question/[action]")]
    [ApiController]
    public class QuestionAPIController : ControllerBase
    {
        private readonly ILogger<QuestionAPIController> _logger;
        private static Random random = new Random();

        public QuestionAPIController(ILogger<QuestionAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var questionModel = await QuestionManager.GetQuestionWithAnswer(questionId);
            return Ok(questionModel);
        }

        [HttpGet("{roomId}")]
        public IActionResult GetRandomQuestionId(int roomId)
        {
            int[] questionIds = QuestionManager.GetAllQuestionIds(roomId);

            if(questionIds.Length == 0)
                return NotFound("No questions found");

            int randomIndex;
            lock(random)
            {
                randomIndex = random.Next(0, questionIds.Length);
            }

            int selectionQuestionId = questionIds[randomIndex];
            return Ok(selectionQuestionId);
        }

        [HttpPost("{questionId}")]
        public async Task<IActionResult> GetFullQuestion([FromBody] int questionId)
        {
            var questionModel = await QuestionManager.GetQuestionWithAnswer(questionId);
            return Ok(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult SaveQuestion(int roomId, [FromBody] QuestionWithAnswerTransferModel questionModel)
        {
            if (questionModel == null)
            {
                return BadRequest("Question data is null.");
            }

            QuestionManager.CreateNewQuestion(roomId, questionModel);
            return Ok("Question created successfully.");
        }
    }
}
