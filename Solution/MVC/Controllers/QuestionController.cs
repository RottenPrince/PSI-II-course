using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using MVC.Models;

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

        [HttpGet("{roomId}")]
        public IActionResult Solve(string roomId)
        {
            var questionName = APIHelper.Get<string>($"api/QuestionAPI/GetRandomQuestionName/{roomId}", out _);
            var questionModel = APIHelper.Get<QuestionModel>($"api/QuestionAPI/GetQuestion/{roomId}/{questionName}", out _);

            ViewBag.RoomId = roomId;

            return View(new SolveViewModel(questionModel, questionName));
        }

        [HttpPost("{roomId}")]
        public IActionResult Solve(string roomId, string questionName, int selectedOption)
        {
            var questionModel = APIHelper.Post<QuestionLocationModel, QuestionModelWithAnswer>("api/QuestionAPI/GetFullQuestion", new QuestionLocationModel(questionName, roomId), out APIError? error);

            //TODO Error handling
            if(questionModel.CorrectAnswerIndex == selectedOption)
            {
                return View("SolveResult", new SolveResultViewModel(questionModel, questionName, roomId));

            }

            return View("SolveResult", new SolveResultViewModel(questionModel, questionName, roomId, wrongAnswerIndex: selectedOption));
        }

        [HttpGet("{roomId}")]
        public IActionResult Create(string roomId)
        {
            var questionModel = new QuestionModelWithAnswer();

            ViewBag.RoomId = roomId;

            return View(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult Create(string roomId, IFormFile? image, QuestionModelWithAnswer questionModel)
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

            var response = APIHelper.Post<QuestionModelWithAnswer, string>($"api/QuestionAPI/SaveQuestion/{roomId}", questionModel, out APIError? error);

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
