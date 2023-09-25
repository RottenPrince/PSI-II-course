using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SolveController : Controller
    {
        public IActionResult Index()
        {
            // TODO handle bad results
            var question = APIHelper.Get<APIResultWithData<string>>("api/SolveAPI/GetRandomQuestionName");
            var questionName = question.Result;

            var questionModel = APIHelper.Get<APIResultWithData<QuestionModel>>($"api/SolveAPI/GetQuestion/{questionName}");

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel.Result);
        }
    }
}
