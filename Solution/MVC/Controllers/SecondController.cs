using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Reflection;

namespace MVC.Controllers
{
    public class SecondController : Controller
    {
        public IActionResult Index()
        {
            var result = APIHelper.Get<Dictionary<int, string>>("api/SecondApi/GetAll");
            return View(result);
        }

        [HttpGet]
        public IActionResult CreateNew()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateNew(int id, string value)
        {
            var result = APIHelper.Post<NewString, APIResult>("api/SecondApi", new NewString(id, value));
            TempData["message"] = result.Success ? "Added successfully" : result.Message;
            return View();
        }

        [HttpGet("GetById")]
        public IActionResult GetById([FromQuery] int? id = null)
        {
            if (id != null)
            {
                var value = APIHelper.Get<APIResultWithData<string>>($"api/SecondApi/{id}");
                if (!value.Success) { TempData["message"] = value.Message; }
                else return View(new Tuple<int, string>((int)id, value.Result));
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete([FromQuery] int? id = null)
        {
            if (id != null)
            {
                var result = APIHelper.Delete<APIResult>($"api/SecondApi/{id}");
                TempData["message"] = result.Success ? "Deleted successfully" : result.Message;
                return View();
            }
            return View();
        }

        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Update(int id, string value)
        {
            var result = APIHelper.Put<NewString, APIResult>("api/SecondApi", new NewString(id, value));
            TempData["message"] = result.Success ? "Updated successfully" : result.Message;
            return View();
        }
    }
}
