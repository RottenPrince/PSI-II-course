using Microsoft.AspNetCore.Mvc;
using BrainBoxUI.Helpers.API;
using SharedModels.Question;
using SharedModels.Lobby;

namespace BrainBoxUI.Controllers
{
    [Route("[controller]/[action]")]
    public class LobbyController : Controller
    {
        private readonly IApiRepository _apiRepository;

        public LobbyController(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        [HttpGet("{roomId}")]
        public IActionResult Room(string roomId)
        {
            var roomContent = _apiRepository.Get<RoomContentDTO>($"api/Lobby/GetRoomContent/{roomId}", includeBearerToken: false, out APIError? error);

            if (error == null)
            {
                ViewBag.RoomName = roomContent.RoomName;
                ViewBag.QuestionAmount = roomContent.QuestionAmount;
                ViewBag.RoomId = roomId;
            }
            else
            {
                ViewBag.ErrorMessage = error.Message;
            }


            return View();
        }

        [HttpGet]
        public IActionResult AllRooms()
        {
            var rooms = _apiRepository.Get<List<RoomDTO>>("api/Lobby/GetAllRooms", includeBearerToken: true, out _);

            return View(rooms);
        }

        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult CreateRoom(string roomName)
        {
            var roomId = _apiRepository.Post<string, int>("api/Lobby/CreateRoom", roomName, includeBearerToken: false, out var error1);
            ViewBag.RoomId = roomId;
            if (error1 != null)
            {
                ViewBag.ErrorMessage = error1.Message;
                return View("CreateError");
            }

            _apiRepository.Get<string>($"api/Lobby/JoinRoom/{roomId}", includeBearerToken: true, out _); //no error handling

            ViewBag.RoomName = roomName;

            return View("CreateSuccess");
        }

        [HttpGet("{roomId}")]
        public IActionResult CreateMultiple(int roomId)
        {
            ViewBag.RoomId = roomId;
            return View("CreateMultiple");
        }

        [HttpPost("{roomId}")]
        public async Task<IActionResult> CreateMultiple(int roomId, IFormFile? questions)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                TempData["message"] = errors.First().ErrorMessage;
                return RedirectToAction("CreateMultiple", "Lobby", new { roomId = roomId });
            }

            if(questions == null)
            {
                TempData["message"] = "No file attached";
                return RedirectToAction("CreateMultiple", "Lobby", new { roomId = roomId });
            }

            byte[] data = new byte[questions.Length];
            using(var stream = questions.OpenReadStream())
            {
                await stream.ReadExactlyAsync(data, 0, (int)questions.Length);
            }

            var string_data = System.Text.Encoding.UTF8.GetString(data);
            var question_objects = string_data.Split(new char[] { '\r', '\n' })
                    .Where(x => x.Length > 0)
                    .Select(x =>
                    {
                        var cells = x.Split(';');
                        // expect question name, correct answer index and at least 2 questions
                        if (cells.Length < 4) throw new Exception("Expected at least 4 columns in each row");
                        var title = cells[0];
                        var correct_ans = int.Parse(cells[1]);

                        var question = new QuestionWithAnswerDTO
                        {
                            Title = title,
                            AnswerOptions = cells.Skip(2).Select(c => new AnswerOptionDTO { OptionText = c }).ToList(),
                            CorrectAnswerIndex = correct_ans,
                            ImageSource = null,
                        };
                        if (correct_ans >= question.AnswerOptions.Count) throw new Exception($"Out of bounds for question {correct_ans}");
                        return question;
                    })
                    .ToList();

            var result = _apiRepository.Post<List<QuestionWithAnswerDTO>, string>($"api/Question/SaveMultipleQuestions/{roomId}", question_objects, true, out var error);

            if(error != null)
            {
                if (error.Message.Length == 0) TempData["message"] = ReasonPhrases.GetReasonPhrase(((int)error.Status));
                else TempData["message"] = error.Message;

                return RedirectToAction("CreateMultiple", "Lobby", new { roomId = roomId });
            }

            TempData["message"] = result;
            return RedirectToAction("CreateMultiple", "Lobby", new { roomId = roomId });
        }
    }
}
