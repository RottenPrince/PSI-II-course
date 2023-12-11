using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxUI.Helpers.API;

namespace BrainBoxUI.Controllers
{
    [Route("[controller]/[action]")]
    public class QuestionController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly ILogger<QuestionController> _logger;
        private readonly IApiRepository _apiRepository;

        public QuestionController(IWebHostEnvironment host, ILogger<QuestionController> logger, IApiRepository apiRepository)
        {
            _host = host;
            _logger = logger;
            _apiRepository = apiRepository;
        }

        [HttpGet("{roomId}/{questionAmount}")]
        public IActionResult StartRun(int roomId, int questionAmount)
        {
            int newRunId = _apiRepository.Get<int>($"api/Lobby/CreateQuiz/{roomId}/{questionAmount}", includeBearerToken: true, out var error);
            if(error != null)
            {
                throw new Exception(); // TODO something normal
            }

            return RedirectToAction("Solve", new { runId = newRunId });
        }

        [HttpGet]
        public IActionResult Solve(int runId)
        {
            var questionModel = _apiRepository.Get<QuestionDTO>($"api/Lobby/GetNextQuestionInQuiz/{runId}", includeBearerToken: true, out var error);
            if(questionModel == null)
            {
                return RedirectToAction("Review", new { runId = runId, currentQuestionIndex = 0 });
            }
            ViewBag.runId = runId;
            var roomId = _apiRepository.Get<int>($"api/Lobby/GetRoomId/{runId}", includeBearerToken: true, out var error2);
            ViewBag.roomId = roomId;

            return View("Solve", questionModel);
        } 

        [HttpPost]
        public IActionResult Solve(int runId, int selectedOption)
        {
            _apiRepository.Post<object, string>($"api/Lobby/SubmitAnswer/{runId}/{selectedOption}", new { }, includeBearerToken: true, out var error);
            return RedirectToAction("Solve", new { runId = runId });
        }

        [HttpGet]
        public IActionResult Review(int runId, int currentQuestionIndex)
        {
            var questionModel = _apiRepository.Get<QuizQuestionDTO>($"api/Lobby/GetNextQuestionInReview/{runId}/{currentQuestionIndex}", includeBearerToken: true, out var error);
            if(questionModel == null)
            {
                var roomId = _apiRepository.Get<int>($"api/Lobby/GetRoomId/{runId}", includeBearerToken: true, out var error2);
                return RedirectToAction("Room", "Lobby", new { roomId = roomId });
            }
            ViewBag.runId = runId;
            ViewBag.currentQuestionIndex = currentQuestionIndex + 1;
            return View("Review", questionModel);
        }

        [HttpGet("{roomId}")]
        public IActionResult Create(string roomId)
        {
            var questionModel = new QuestionWithAnswerDTO();

            ViewBag.RoomId = roomId;

            return View(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult Create(string roomId, IFormFile? image, QuestionWithAnswerDTO questionModel)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                ViewBag.ErrorMessage = errors.First().ErrorMessage;
                return View("CreateError");
            }

            if(image != null)
            {
                string extension = Path.GetExtension(image.FileName);
                string guid = Guid.NewGuid().ToString();
                string filename = $"{guid}.{extension}";
                string savedImagePath = Path.Combine(_host.WebRootPath, "img", filename);
                questionModel.ImageSource = filename;

                using (FileStream fs = System.IO.File.Create(savedImagePath))
                {
                    image.CopyTo(fs);
                }
            }

            ViewBag.Title = questionModel.Title;

            var response = _apiRepository.Post<QuestionWithAnswerDTO, string>($"api/Question/SaveQuestion/{roomId}", questionModel, includeBearerToken: true, out APIError? error);

            if (error == null)
            {
                ViewBag.RoomId = roomId;
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
