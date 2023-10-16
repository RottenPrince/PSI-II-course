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

        [HttpGet("GetQuestion/{room}/{question}")]
        public IActionResult GetQuestion(string room, string question)
        {
            var questionModel = QuestionManager.GetQuestionWithoutAnswer(room, question, out var error);
            if (error != null) return error;
            return Ok(questionModel);
        }

        [HttpGet("GetRandomQuestionName/{room}")]
        public IActionResult GetRandomQuestionName(string room)
        {
            string[] questionFiles = QuestionManager.GetAllQuestionNames(room);

            if(questionFiles.Length == 0)
                return NotFound("No questions found");

            int randomIndex;
            lock(random)
            {
                randomIndex = random.Next(0, questionFiles.Length);
            }

            string selectionQuestion = questionFiles[randomIndex];
            return Ok(selectionQuestion);
        }

        [HttpPost("{roomId}")]
        public IActionResult GetFullQuestion([FromBody] QuestionLocationModel model)
        {
            var questionModel = QuestionManager.GetQuestionWithAnswer(model.RoomId, model.Name, out var error);
            if (error != null)
                return error;

            return Ok(questionModel);
        }

        [HttpPost("SaveQuestion/{room}")]
        public IActionResult SaveQuestion(string room, [FromBody] QuestionModelWithAnswer questionModel)
        {
            if (questionModel == null)
            {
                return BadRequest("Question data is null.");
            }

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string questionName = $"question_{uniqueIdentifier}";
            QuestionManager.CreateNewQuestion(room, questionName, questionModel);

            return Ok("Question created successfully.");
        }
    }
}
