using API.Models.Question;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SolveController : Controller
    {
		private readonly ILogger<SolveController> _logger;

		public SolveController(ILogger<SolveController> logger)
		{
			_logger = logger;
		}
		
        public IActionResult Index()
        {
            var questionName = APIHelper.Get<string>("api/SolveAPI/GetRandomQuestionName", out _);
			questionName = "uruguay";
            var questionModel = APIHelper.Get<BaseQuestionModel>($"api/SolveAPI/GetQuestion/{questionName}", out _);

			_logger.LogInformation($"{questionModel}");

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel);
        }
    }
}
