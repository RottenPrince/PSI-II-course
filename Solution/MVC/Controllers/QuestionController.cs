using Microsoft.AspNetCore.Mvc;
using SharedModels.Question.WithAnswer;
using SharedModels.Question.WithoutAnswer;

namespace MVC.Controllers
{
    public class QuestionController : Controller
    {
		private readonly ILogger<QuestionController> _logger;

		public QuestionController(ILogger<QuestionController> logger)
		{
			_logger = logger;
		}
		
		[HttpGet("/Question/Solve/{questionName}")]
        public IActionResult Solve(string questionName)
        {
            var questionModel = APIHelper.Get<BaseQuestionModel>($"api/QuestionAPI/GetQuestion/{questionName}", out var err);
			if(err != null)
			{
				_logger.LogError($"Failure: {err.Status} {err.Message}");
				return View("Error");
			}

            ViewData["QuestionName"] = questionName;
            return View(questionModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var questionModel = new BaseQuestionWithAnswerModel();

            return View(questionModel);
        }

        [HttpPost]
        public IActionResult Create(BaseQuestionWithAnswerModel sm)
        {
            ViewBag.Title = sm.Question;

            var response = APIHelper.Post<BaseQuestionWithAnswerModel, string>("api/QuestionAPI/SaveQuestion", sm, out APIError? error);

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
