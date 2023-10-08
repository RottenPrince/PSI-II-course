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

            return View(new SolveViewModel(questionModel, questionName, roomId));
        }

        [HttpGet("{roomId}")]
        public IActionResult Create()
        {
            var questionModel = new QuestionModelWithAnswer();

            return View(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult Create(string roomId, IFormFile image, QuestionModelWithAnswer questionModel)
        {
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
