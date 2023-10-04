using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class QuestionController : Controller
    {
        public IActionResult Solve()
        {
            var questionName = APIHelper.Get<string>("api/QuestionAPI/GetRandomQuestionName", out _);
            var questionModel = APIHelper.Get<QuestionModel>($"api/QuestionAPI/GetQuestion/{questionName}", out _);

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel);
        }

        [HttpGet("Create/{room}")]
        public IActionResult Create(string room)
        {
            var questionModel = new QuestionModelWithAnswer();

            return View(questionModel);
        }

        [HttpPost("Create/{room}")]
        public IActionResult Create(string room, QuestionModelWithAnswer questionModel)
        {
            
            ViewBag.Title = questionModel.Question;

            var response = APIHelper.Post<QuestionModelWithAnswer, string>($"api/QuestionAPI/SaveQuestion/{room}", questionModel, out APIError? error);

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
