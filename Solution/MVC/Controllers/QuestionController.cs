using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

            return View(new SolveDisplayModel(questionModel, questionName, room));
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
            ViewBag.Title = questionModel.Title;

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
