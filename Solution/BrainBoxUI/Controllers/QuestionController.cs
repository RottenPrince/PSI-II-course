using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using BrainBoxUI.Helpers.BrainBoxAPI;
using BrainBoxUI.Models;
using System.Text.Json;
using Microsoft.Identity.Client;

namespace BrainBoxUI.Controllers
{
    [Route("[controller]/[action]")]
    public class QuestionController : Controller
    {
        private readonly IWebHostEnvironment _host;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(IWebHostEnvironment host, ILogger<QuestionController> logger)
        {
            _host = host;
            _logger = logger;
        }

        [HttpGet("{roomId}/{questionAmount}")]
        public IActionResult StartRun(int roomId, int questionAmount)
        {
            int newRunId = APIHelper.Get<int>($"api/Lobby/CreateSolveRun/{roomId}/{questionAmount}", out var error);
            if(error != null)
            {
                throw new Exception(); // TODO something normal
            }

            return RedirectToAction("Solve", new { runId = newRunId });
        }

        [HttpGet]
        public IActionResult Solve(int runId)
        {
            var questionModel = APIHelper.Get<QuestionTransferModel>($"api/Lobby/GetNextQuestionInRun/{runId}", out var error);
            if(questionModel == null)
            {
                return RedirectToAction("Review", new { runId = runId, currentQuestionIndex = 0 });
            }
            ViewBag.runId = runId;
            var roomId = APIHelper.Get<int>($"api/Lobby/GetRoomId/{runId}", out var error2);
            ViewBag.roomId = roomId;

            return View("Solve", questionModel);
        } 

        [HttpPost]
        public IActionResult Solve(int runId, int selectedOption)
        {
            APIHelper.Post<object, string>($"api/Lobby/SubmitAnswer/{runId}/{selectedOption}", new { }, out var error);
            return RedirectToAction("Solve", new { runId = runId });
        }

        [HttpGet]
        public IActionResult Review(int runId, int currentQuestionIndex)
        {
            var questionModel = APIHelper.Get<QuestionRunTransferModel>($"api/Lobby/GetNextQuestionInReview/{runId}/{currentQuestionIndex}", out var error);
            if(questionModel == null)
            {
                var roomId = APIHelper.Get<int>($"api/Lobby/GetRoomId/{runId}", out var error2);
                return RedirectToAction("Room", "Lobby", new { roomId = roomId });
            }
            ViewBag.runId = runId;
            ViewBag.currentQuestionIndex = currentQuestionIndex + 1;
            return View("Review", questionModel);
        }

        [HttpGet("{roomId}")]
        public IActionResult Create(string roomId)
        {
            var questionModel = new QuestionWithAnswerTransferModel();

            ViewBag.RoomId = roomId;

            return View(questionModel);
        }

        [HttpPost("{roomId}")]
        public IActionResult Create(string roomId, IFormFile? image, QuestionWithAnswerTransferModel questionModel)
        {
            Console.WriteLine(questionModel.AnswerOptions.Count);

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
                questionModel.ImageName = filename;

                using(FileStream fs = System.IO.File.Create(savedImagePath))
                {
                    image.CopyTo(fs);
                }
            }

            ViewBag.Title = questionModel.Title;

            var response = APIHelper.Post<QuestionWithAnswerTransferModel, string>($"api/Question/SaveQuestion/{roomId}", questionModel, out APIError? error);

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
