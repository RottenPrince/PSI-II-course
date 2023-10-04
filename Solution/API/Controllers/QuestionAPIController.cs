using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using API.Databases;
using SharedModels.Question.WithoutAnswer;

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

        [HttpGet("GetQuestion/{question}")]
        public IActionResult GetQuestion(string question)
        {
            var questionModel = QuestionDatabase.GetQuestionWithoutAnswer(question, out var error);
            if (error != null) return error;
            return Ok(questionModel);
        }

        [HttpGet("GetRandomQuestionName")]
        public IActionResult GetRandomQuestionName()
        {
            string[] questionFiles = QuestionDatabase.GetAllQuestionNames();

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

        [HttpPost("CheckAnswer")]
        public IActionResult CheckAnswer([FromBody] CheckAnswerModel model)
        {
            var questionModel = QuestionDatabase.GetQuestionWithAnswer(model.Name, out var error);
            if (error != null)
                return error;

            return Ok(questionModel.Validate(model.Answer));
        }

        [HttpPost("SaveQuestion")]
        public IActionResult SaveQuestion([FromBody] BaseQuestionWithAnswerModel questionModel)
        {
            if (questionModel == null)
            {
                return BadRequest("Question data is null.");
            }

            string uniqueIdentifier = Guid.NewGuid().ToString();
            string questionName = $"question_{uniqueIdentifier}";
            QuestionDatabase.CreateNewQuestion(questionName, questionModel);

            return Ok("Question created successfully.");
        }
    }
}
