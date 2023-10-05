using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels.Question.WithoutAnswer;
using SharedModels.Question.WithAnswer;
using System.Text.Json;

namespace API.Databases
{
    public static class QuestionDatabase
    {
        private static readonly string _questionsFolder = "../../questions";
        private static readonly string _questionDBExtension = ".json";

        public static BaseQuestionModel? GetQuestionWithoutAnswer(string questionName, out ActionResult? error)
        {
            // return ParseQuestionFromDatabase<BaseQuestionModel>(questionName, out error);
            error = null;
            return null;
        }
        
        public static BaseQuestionWithAnswerModel? GetQuestionWithAnswer(string questionName, out ActionResult? error)
        {
            return ParseQuestionFromDatabase<BaseQuestionWithAnswerModel>(questionName, out error);
        }

        public static string[] GetAllQuestionNames()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(_questionsFolder);
            return (from file in dirinfo.GetFiles()
                    where file.Name.EndsWith(_questionDBExtension)
                    select file.Name.Substring(0, file.Name.Length - _questionDBExtension.Length))
                .ToArray();
        }

        public static void CreateNewQuestion(string questionName, BaseQuestionWithAnswerModel questionModel)
        {
            string jsonQuestion = JsonConvert.SerializeObject(questionModel);
            string fileName = $"{questionName}{_questionDBExtension}";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(_questionsFolder, fileName)))
            {
                outputFile.Write(jsonQuestion);
            }
        }

        private static T? ParseQuestionFromDatabase<T>(string question, out ActionResult? error) where T: BaseQuestionWithAnswerModel
        {
            if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
            {
                error = new BadRequestObjectResult("Question name should only contain alphanumeric characters, dashes and underscores");
                return null;
            }

            string questionModelFile = Path.Combine(_questionsFolder, question + ".json");

            if (!System.IO.File.Exists(questionModelFile))
            {
                error = new NotFoundObjectResult("Question not found with given name");
                return null;
            }

            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

            var questionModel = System.Text.Json.JsonSerializer.Deserialize<T>(questionModelText);
            if(questionModel == null)
            {
                error = new UnprocessableEntityObjectResult("Failed deserializing question");
                return null;
            }

            error = null;
            return questionModel;
        }
    }
}
