using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SolveController : Controller
    {
        public IActionResult Index()
        {
            // TODO handle bad results
            var questionName = APIHelper.Get<String>("api/SolveAPI/GetRandomQuestionName");
            var questionModel = APIHelper.Get<QuestionModel>($"api/SolveAPI/GetQuestion/{questionName}");

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel);
        }
    }
}
