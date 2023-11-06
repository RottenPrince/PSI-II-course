using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using MVC.Models;
using System.Text.Json;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class QuestionController : Controller
    {
        private readonly IWebHostEnvironment _host;

        public QuestionController(IWebHostEnvironment host)
        {
            _host = host;
        }

        //[HttpPost("{roomId}")]
        //public IActionResult Solve(string roomId, int questionId, int selectedOption)
        //{
        //    var questionModel = APIHelper.Get<QuestionWithAnswerTransferModel>($"api/Question/GetFullQuestion/{questionId}", out APIError? error);
        //    //TODO Error handling

        //    if(questionModel.CorrectAnswerIndex == selectedOption)
        //    {
        //        return View("SolveResult", new SolveResultViewModel(questionModel, questionId, roomId)); //questionId possibly not needed

        //    }

        //    return View("SolveResult", new SolveResultViewModel(questionModel, questionId, roomId, wrongAnswerIndex: selectedOption));
        //}

        [HttpPost]
        public IActionResult Solve(int roomId, int currentQuestionIndex, string questionsIdJson, string answersJson, int selectedOption)
        {
            var questionsId = JsonSerializer.Deserialize<List<int>>(questionsIdJson);
            var answers = JsonSerializer.Deserialize<List<int>>(answersJson);

            if (questionsId.Count == 0)
                return View("NoQuestionsAvailable");
            SolveRunModel runModel = new SolveRunModel(questionsId);
            runModel.answers = answers;
            runModel.answers.Add(selectedOption);
            runModel.currentQuestionIndex = ++currentQuestionIndex;
            
            if (runModel.currentQuestionIndex == runModel.questionsId.Count)
                return View("NoQuestionsAvailable");

            var newQuestionModel = APIHelper.Get<QuestionTransferModel>($"api/Question/GetQuestion/{runModel.questionsId[runModel.currentQuestionIndex]}", out _);
            runModel.currentQuestion = newQuestionModel;

            ViewBag.RoomId = roomId;
            return View("Solve", runModel);
        }


        [HttpGet("{roomId}/{questionAmount}")]
        public IActionResult StartRun(int roomId, int questionAmount)
        {
            var questionsForRun = APIHelper.Get<List<int>>($"api/Question/GetQuestionList/{roomId}/{questionAmount}", out _);
            
            if(questionsForRun == null || questionsForRun.Count == 0)
            {
                ViewBag.RoomId = roomId;
                return View("NoQuestionsAvailable");
            }

            SolveRunModel runModel = new SolveRunModel(questionsForRun);

            var currentQuestion = APIHelper.Get<QuestionTransferModel>($"api/Question/GetQuestion/{questionsForRun[0]}", out _);

            runModel.currentQuestion = currentQuestion;
            ViewBag.RoomId = roomId;

            return View("Solve", runModel);
        }

        [HttpGet("{roomId}")]
        public IActionResult Create(string roomId)
        {
            var questionModel = new QuestionWithAnswerTransferModel();

            ViewBag.RoomId = roomId;

            return View(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult Create(string roomId, IFormFile? image, QuestionWithAnswerTransferModel questionModel)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                ViewBag.ErrorMessage = errors.First().ErrorMessage;
                return View("CreateError");
            }

            if(image != null)
            {
                string extension = Path.GetExtension(image.FileName);
                string guid = Guid.NewGuid().ToString();
                string filename = $"{guid}.{extension}";
                string savedImagePath = Path.Combine(_host.WebRootPath, "img", filename);
                questionModel.ImageName = filename;

                using(FileStream fs = System.IO.File.Create(savedImagePath))
                {
                    image.CopyTo(fs);
                }
            }

            ViewBag.Title = questionModel.Title;

            var response = APIHelper.Post<QuestionWithAnswerTransferModel, string>($"api/Question/SaveQuestion/{roomId}", questionModel, out APIError? error);

            if (error == null)
            {
                ViewBag.RoomId = roomId;
                return View("CreateSuccess");
            }
            else
            {
                ViewBag.ErrorMessage = error.Message;
                return View("CreateError");
            }
        }
    }
}
