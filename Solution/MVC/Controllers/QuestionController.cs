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
    }
}
