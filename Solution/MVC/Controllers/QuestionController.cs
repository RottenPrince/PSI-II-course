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

        [HttpGet]
        public IActionResult Create()
        {
            var questionModel = new BaseQuestionModel();

            return View(questionModel);
        }

        [HttpPost]
        public IActionResult Create(IVerifiable sm)
        {
            var questionModel = (BaseQuestionModel)sm;
            ViewBag.Title = questionModel.Question;

            var response = APIHelper.Post<IVerifiable, string>("api/QuestionAPI/SaveQuestion", sm, out APIError? error);

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
