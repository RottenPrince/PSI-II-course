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
		
		[HttpGet("Solve/{questionName}")]
        public IActionResult Index(string questionName)
        {
            var questionModel = APIHelper.Get<BaseQuestionModel>($"api/SolveAPI/GetQuestion/{questionName}", out var err);
			if(err != null)
			{
				_logger.LogError($"Failure: {err.Status} {err.Message}");
				return View("Error");
			}

            ViewData["QuestionName"] = questionName; // TODO I don't like this
            return View(questionModel);
        }
    }
}
