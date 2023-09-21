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
		private static Random random = new Random();

        public SolveAPIController(ILogger<SolveAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetQuestion/{question}")]
        public APIResultWithData<QuestionModel> GetQuestion(string question)
        {
            // string[] imageFiles = Directory.GetFiles(questionFolder, "*.jpg");

            // if (imageFiles.Length == 0)
            //     return NotFound("No image file found in the selected question subfolder");

            // string questionImageFile = imageFiles[0]; // Assuming only one image file per subfolder
            // byte[] imageBytes = System.IO.File.ReadAllBytes(questionImageFile);

            // Find the corresponding answer text file (assuming the name matches)

			// verify all question characters are alphanumeric
			if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
				return APIResultWithData<QuestionModel>.CreateFailure("Question name should only contain alphanumeric characters, dashes and underscores");

            string questionModelFile = Path.Combine(_questionsFolder, question, "question.json");

            if (!System.IO.File.Exists(questionModelFile))
                return APIResultWithData<QuestionModel>.CreateFailure("Answer file not found for the selected question");

            // Read the answer from the text file
            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

			var questionModel = JsonConvert.DeserializeObject<QuestionModel>(questionModelText);
			return APIResultWithData<QuestionModel>.CreateSuccess(questionModel);
        }

		[HttpGet("GetRandomQuestionName")]
		public APIResultWithData<string> GetRandomQuestionName()
		{
			string[] questionSubfolders = Directory.GetDirectories(_questionsFolder).Select(s => new DirectoryInfo(s).Name).ToArray();

			if(questionSubfolders.Length == 0)
				return APIResultWithData<string>.CreateFailure("No questions found");

			int randomIndex;
			lock(random)
			{
				randomIndex = random.Next(0, questionSubfolders.Length);
			}

			string questionFolder = questionSubfolders[randomIndex];
			return APIResultWithData<string>.CreateSuccess(questionFolder);
		}

		[HttpPost("CheckAnswer")]
		public APIResultWithData<bool> CheckAnswer([FromBody] CheckAnswerModel model)
		{
			var result = this.GetQuestion(model.name);
			if(!result.Success)
				return APIResultWithData<bool>.CreateFailure(result.Message);

			return APIResultWithData<bool>.CreateSuccess(result.Result.CorrectAnswer == model.answer);
		}
    }
}
