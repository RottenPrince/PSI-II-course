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
        public IActionResult Solve(int roomId)
        {
            var questionId = APIHelper.Get<int>($"api/Question/GetRandomQuestionId/{roomId}", out _);
            var questionModel = APIHelper.Get<QuestionTransferModel>($"api/Question/GetQuestion/{questionId}", out _);

            ViewBag.RoomId = roomId;

            return View(new SolveViewModel(questionModel, questionId));
        }

        [HttpPost("{roomId}")]
        public IActionResult Solve(string roomId, int questionId, int selectedOption)
        {
            var questionModel = APIHelper.Post<int, QuestionWithAnswerTransferModel>("api/Question/GetFullQuestion", questionId, out APIError? error);

            //TODO Error handling
            if(questionModel.CorrectAnswerIndex == selectedOption)
            {
                return View("SolveResult", new SolveResultViewModel(questionModel, questionId, roomId)); //questionId possibly not needed

            }

            return View("SolveResult", new SolveResultViewModel(questionModel, questionId, roomId, wrongAnswerIndex: selectedOption));
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
