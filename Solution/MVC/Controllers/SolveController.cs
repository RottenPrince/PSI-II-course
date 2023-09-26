using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SolveController : Controller
    {
        public IActionResult Index()
        {
            var questionName = APIHelper.Get<string>("api/SolveAPI/GetRandomQuestionName", out _);
            var questionModel = APIHelper.Get<QuestionModel>($"api/SolveAPI/GetQuestion/{questionName}", out _);

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel);
        }
    }
}
