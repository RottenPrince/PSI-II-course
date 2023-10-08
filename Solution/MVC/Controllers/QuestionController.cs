using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using MVC.Models;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class QuestionController : Controller
    {
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
        public IActionResult Create(string roomId, QuestionModelWithAnswer questionModel)
        {
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
