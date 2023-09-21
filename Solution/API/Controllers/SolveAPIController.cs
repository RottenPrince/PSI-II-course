using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using Newtonsoft.Json;

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
            string questionModelFile = Path.Combine(questionFolder, "question.json");

            if (!System.IO.File.Exists(questionModelFile))
                return NotFound("Answer file not found for the selected question");

            // Read the answer from the text file
            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

			var questionModel = JsonConvert.DeserializeObject<QuestionModel>(questionModelText);

            return Ok(questionModel); // Return the question as JSON with image data
        }
    }
}
