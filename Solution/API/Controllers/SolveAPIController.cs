﻿using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Models.Question;
using System.Text.Json;

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

        private static ObjectResult ParseQuestionFromDatabase(string question)
        {
            if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
                return new BadRequestObjectResult("Question name should only contain alphanumeric characters, dashes and underscores");

            string questionModelFile = Path.Combine(_questionsFolder, question + ".json");

            if (!System.IO.File.Exists(questionModelFile))
                return new NotFoundObjectResult("Question not found with given name");

            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

            var questionModel = JsonSerializer.Deserialize<BaseQuestionModel>(questionModelText);
            if(questionModel == null)
                return new UnprocessableEntityObjectResult("Failed deserializing question");

            return new OkObjectResult(questionModel);
        }

        [HttpGet("GetQuestion/{question}")]
        public IActionResult GetQuestion(string question)
        {
            return ParseQuestionFromDatabase(question);
        }

        [HttpGet("GetRandomQuestionName")]
        public IActionResult GetRandomQuestionName()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(_questionsFolder);
            string[] questionFiles = (from file in dirinfo.GetFiles()
                    where file.Name.EndsWith(_questionDBExtension)
                    select file.Name.Substring(0, file.Name.Length - _questionDBExtension.Length))
                .ToArray();


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
			var result = ParseQuestionFromDatabase(model.QuestionName);
			if(result.StatusCode != 200)
				return result;

			var questionModel = (BaseQuestionModel)result.Value;
            return Ok(questionModel.Validate(model.Answer));
        }
    }
}
