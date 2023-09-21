using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SolveController : Controller
    {
        public IActionResult Index()
        {
            
            var question = APIHelper.Get<QuestionModel>("api/SolveAPI/GetQuestion");
            return View(question);
        }
    }
}
