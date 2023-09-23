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
		private static readonly string _questionDBExtension = ".json";
        private readonly ILogger<SolveAPIController> _logger;
		private static Random random = new Random();

        public SolveAPIController(ILogger<SolveAPIController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetQuestion/{question}")]
        public APIResultWithData<QuestionModel> GetQuestion(string question)
        {
			if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
				return APIResultWithData<QuestionModel>.CreateFailure("Question name should only contain alphanumeric characters, dashes and underscores");

            string questionModelFile = Path.Combine(_questionsFolder, question + ".json");

            if (!System.IO.File.Exists(questionModelFile))
                return APIResultWithData<QuestionModel>.CreateFailure("Answer file not found for the selected question");

            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

			var questionModel = JsonConvert.DeserializeObject<QuestionModel>(questionModelText);
			return APIResultWithData<QuestionModel>.CreateSuccess(questionModel);
        }

		[HttpGet("GetRandomQuestionName")]
		public APIResultWithData<string> GetRandomQuestionName()
		{
			DirectoryInfo dirinfo = new DirectoryInfo(_questionsFolder);
			string[] questionFiles = (from file in dirinfo.GetFiles()
												where file.Name.EndsWith(_questionDBExtension)
												select file.Name.Substring(0, file.Name.Length - _questionDBExtension.Length))
												.ToArray();


			if(questionFiles.Length == 0)
				return APIResultWithData<string>.CreateFailure("No questions found");

			int randomIndex;
			lock(random)
			{
				randomIndex = random.Next(0, questionFiles.Length);
			}

			string questionFolder = questionFiles[randomIndex];
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
