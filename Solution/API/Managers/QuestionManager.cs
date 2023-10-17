using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels.Question;
using SharedModels.Lobby;
using API.Enums.QuestionManager;

namespace API.Managers
{
    public static class QuestionManager
    {
        private static readonly string _questionsFolder = "../../questions";
        private static readonly string _questionDBExtension = ".json";

        public static QuestionModel? GetQuestionWithoutAnswer(string room, string questionName, out QuestionParsingError? error)
        {
            return ParseQuestionFromDatabase<QuestionModel>(room, questionName, out error);
        }

        public static QuestionModelWithAnswer? GetQuestionWithAnswer(string roomId, string questionName, out QuestionParsingError? error)
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

        public static List<QuestionModelWithAnswer?> GetAllQuestions(string room)
        {
            var questions = GetAllQuestionNames(room)
                            .Select(x => GetQuestionWithAnswer(room, x, out _))
                            .ToList();

            questions.Sort();
            return questions;
        }

        public static int GetQuestionCountInRoom(string room)
        {
            return GetAllQuestionNames(room).Length;
        }

        public static void CreateNewQuestion(string room, string questionName, QuestionModelWithAnswer questionModel)
        {
            int newQuestionIndex = GetQuestionCountInRoom(room);
            questionModel.QuestionIndex = newQuestionIndex;

            string jsonQuestion = JsonConvert.SerializeObject(questionModel);
            string fileName = $"{questionName}{_questionDBExtension}";

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(_questionsFolder, room, fileName)))
            {
                outputFile.Write(jsonQuestion);
            }
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

        public static IEnumerable<RoomModel> GetAllRooms()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(_questionsFolder);
            return dirinfo.GetDirectories()
                .Select(d => new RoomModel
                {
                    Id = d.Name,
                    Name = File.ReadAllText(Path.Combine(d.FullName, "room.txt")).Trim()
                });
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

            int questionAmount = GetQuestionCountInRoom(room);

            return new RoomContentModel(questionAmount, roomName);
        }

        private static T? ParseQuestionFromDatabase<T>(string room, string question, out QuestionParsingError? error) where T: QuestionModel
        {
            if(!question.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-'))
            {
				error = QuestionParsingError.DisallowedCharacterInName;
                return null;
            }

            string questionModelFile = Path.Combine(_questionsFolder, room, question + ".json");

            if (!System.IO.File.Exists(questionModelFile))
            {
				error = QuestionParsingError.QuestionNotFound;
                return null;
            }

            string questionModelText = System.IO.File.ReadAllText(questionModelFile);

            var questionModel = JsonConvert.DeserializeObject<T>(questionModelText);
            if(questionModel == null)
            {
				error = QuestionParsingError.FailedDeserialization;
                return null;
            }

            error = null;
            return questionModel;
        }
    }
}
