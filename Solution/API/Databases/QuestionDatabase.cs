using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels.Question;
using SharedModels.Lobby;

namespace API.Databases
{
    public static class QuestionDatabase
    {
        private static readonly string _questionsFolder = "../../questions";
        private static readonly string _questionDBExtension = ".json";

        public static QuestionModel? GetQuestionWithoutAnswer(string room, string questionName, out ActionResult? error)
        {
            return ParseQuestionFromDatabase<QuestionModel>(room, questionName, out error);
        }

        public static QuestionModelWithAnswer? GetQuestionWithAnswer(string roomId, string questionName, out ActionResult? error)
        {
            return ParseQuestionFromDatabase<QuestionModelWithAnswer>(roomId, questionName, out error);
        }

        public static string[] GetAllQuestionNames(string room)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(_questionsFolder + "/" + room);
            return (from file in dirinfo.GetFiles()
                    where file.Name.EndsWith(_questionDBExtension)
                    select file.Name.Substring(0, file.Name.Length - _questionDBExtension.Length))
                .ToArray();
        }

        public static void CreateNewQuestion(string room, string questionName, QuestionModelWithAnswer questionModel)
        {
            string jsonQuestion = JsonConvert.SerializeObject(questionModel);
            string fileName = $"{questionName}{_questionDBExtension}";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(_questionsFolder, room, fileName)))
            {
                outputFile.Write(jsonQuestion);
            }
        }

        private static T? ParseQuestionFromDatabase<T>(string room, string question, out ActionResult? error) where T: QuestionModel
        {
            if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
            {
                error = new BadRequestObjectResult("Question name should only contain alphanumeric characters, dashes and underscores");
                return null;
            }

            string questionModelFile = Path.Combine(_questionsFolder, room, question + ".json");

            if (!System.IO.File.Exists(questionModelFile))
            {
                error = new NotFoundObjectResult("Question not found with given name");
                return null;
            }

            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

            var questionModel = JsonConvert.DeserializeObject<T>(questionModelText);
            if(questionModel == null)
            {
                error = new UnprocessableEntityObjectResult("Failed deserializing question");
                return null;
            }

            error = null;
            return questionModel;
        }

        public static string? GetRoomName(string room, out ActionResult? error)
        {
            string roomNameFile = Path.Combine(_questionsFolder, room, "room.txt");

            if (!System.IO.File.Exists(roomNameFile))
            {
                error = new NotFoundObjectResult("Room name file not found");
                return null;
            }

            error = null;
            return  System.IO.File.ReadAllText(roomNameFile);
        }

        public static RoomContentModel? GetRoomContent(string room, out ActionResult? error)
        {
            string roomNameFile = Path.Combine(_questionsFolder, room, "room.txt");

            if (!System.IO.File.Exists(roomNameFile))
            {
                error = new NotFoundObjectResult("Room name file not found");
                return null;
            }

            error = null;

            string roomName = System.IO.File.ReadAllText(roomNameFile);
            
            string roomPath = Path.Combine(_questionsFolder, room);
            string[] questionFiles = Directory.GetFiles(roomPath, "question*.json");
            int questionAmount = questionFiles.Length;

            return new RoomContentModel(questionAmount, roomName);
        }
    }
}
