using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolveAPIController : ControllerBase
    {
        private static readonly string _questionsFolder = "../../questions";

        private readonly ILogger<SolveAPIController> _logger;

        public SolveAPIController(ILogger<SolveAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetQuestion")]
        public IActionResult GetQuestion()
        {
            // Get a list of subdirectories (each subdirectory is a question)
            string[] questionSubfolders = Directory.GetDirectories(_questionsFolder);

            if (questionSubfolders.Length == 0)
                return NotFound("No questions found");

            // Choose a random subfolder (question) from the list
            Random random = new Random();
            int randomIndex = random.Next(0, questionSubfolders.Length);
            string questionFolder = questionSubfolders[randomIndex];

            // Find the image file in the subfolder
            string[] imageFiles = Directory.GetFiles(questionFolder, "*.jpg");

            if (imageFiles.Length == 0)
                return NotFound("No image file found in the selected question subfolder");

            // Read the image data into a byte array
            string questionImageFile = imageFiles[0]; // Assuming only one image file per subfolder
            byte[] imageBytes = System.IO.File.ReadAllBytes(questionImageFile);

            // Find the corresponding answer text file (assuming the name matches)
            string answerTextFile = Path.Combine(questionFolder, "Answer.txt");

            if (!System.IO.File.Exists(answerTextFile))
                return NotFound("Answer file not found for the selected question");

            // Read the answer from the text file
            string answerText = System.IO.File.ReadAllText(answerTextFile);

            // Parse the answer text to an integer
            if (!int.TryParse(answerText, out int answer))
                return BadRequest("Invalid answer format");

            // Create a QuestionModel object with the image data and answer
            var questionModel = new QuestionModel
			{
				Question = "Test Question",
				AnswerOptions = new List<string>{ "A", "B", "C", "D", "E" },
				ImageName = null,
				CorrectAnswer = 2,
			};

            return Ok(questionModel); // Return the question as JSON with image data
        }
    }
}
