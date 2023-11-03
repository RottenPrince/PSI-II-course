using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Managers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionAPIController : ControllerBase
    {
        private readonly ILogger<QuestionAPIController> _logger;
        private static Random random = new Random();

        public QuestionAPIController(ILogger<QuestionAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetQuestion/{roomId}/{questionId}")]
        public IActionResult GetQuestion(int roomId, int questionInd)
        {
            var questionModel = QuestionManager.GetQuestionWithAnswer(roomId);
            return Ok(questionModel);
        }

        [HttpGet("GetRandomQuestionName/{roomId}")]
        public IActionResult GetRandomQuestionName(int roomId)
        {
            int[] questionIds = QuestionManager.GetAllQuestionIds(roomId);

            if(questionIds.Length == 0)
                return NotFound("No questions found");

            int randomIndex;
            lock(random)
            {
                randomIndex = random.Next(0, questionIds.Length);
            }

            int selectionQuestion = questionIds[randomIndex];
            return Ok(selectionQuestion);
        }

        [HttpPost("{questionId}")]
        public IActionResult GetFullQuestion([FromBody] int questionId)
        {
            var questionModel = QuestionManager.GetQuestionWithAnswer(questionId);
            return Ok(questionModel);
        }

        [HttpPost("SaveQuestion/{roomId}")]
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
