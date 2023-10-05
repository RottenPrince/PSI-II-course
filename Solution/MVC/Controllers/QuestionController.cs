using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    [Route("[controller]/[action]/{id}")]
    public class QuestionController : Controller
    {
        [HttpGet]
        public IActionResult Solve(string id)
        {
            string room = id;
            var questionName = APIHelper.Get<string>($"api/QuestionAPI/GetRandomQuestionName/{room}", out _);
            var questionModel = APIHelper.Get<QuestionModel>($"api/QuestionAPI/GetQuestion/{room}/{questionName}", out _);

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            ViewData["Room"] = room; // You don't like it, but here's more
            return View(questionModel);
        }

        [HttpGet]
        public IActionResult Create(string id)
        {
            var questionModel = new QuestionModelWithAnswer();

            return View(questionModel);
        }

        [HttpPost]
        public IActionResult Create(string id, QuestionModelWithAnswer questionModel)
        {
            string room = id;
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
