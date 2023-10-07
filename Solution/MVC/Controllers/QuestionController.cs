using SharedModels.Question;
using Microsoft.AspNetCore.Mvc;
using MVC.Helpers.API;
using MVC.Models;

namespace MVC.Controllers
{
    [Route("[controller]/[action]")]
    public class QuestionController : Controller
    {
        [HttpGet("{roomName}")]
        public IActionResult Solve(string roomName)
        {
            var questionName = APIHelper.Get<string>($"api/QuestionAPI/GetRandomQuestionName/{roomName}", out _);
            var questionModel = APIHelper.Get<QuestionModel>($"api/QuestionAPI/GetQuestion/{roomName}/{questionName}", out _);

            return View(new QuestionSolveViewModel
            {
                QuestionName = questionName,
                RoomName = roomName,
                QuestionModel = questionModel,
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var questionModel = new QuestionModelWithAnswer();

            return View(questionModel);
        }

        [HttpGet("{roomName}")]
        public IActionResult Create(string roomName, QuestionModelWithAnswer questionModel)
        {
            ViewBag.Title = questionModel.Question;

            var response = APIHelper.Post<QuestionModelWithAnswer, string>($"api/QuestionAPI/SaveQuestion/{roomName}", questionModel, out APIError? error);

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
