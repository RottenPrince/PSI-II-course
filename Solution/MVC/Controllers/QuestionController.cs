using API.Models;
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

        [HttpGet]
        public IActionResult Create()
        {
            var questionModel = new QuestionModel();

            return View(questionModel);
        }

        [HttpPost]
        public IActionResult Create(QuestionModel sm)
        {
            
            ViewBag.Title = sm.Question;

            return View("CreateSuccess");
        }
    }
}
