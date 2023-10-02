using API.Models;
using Microsoft.AspNetCore.Mvc;
using MVC;

public class TimerController : Controller
{
    [HttpGet]
    public IActionResult StartTimer(int timerId)
    {
       
        return RedirectToAction("Solve");
    }

    [HttpGet]
    public IActionResult StopTimer(int timerId)
    {
   
        return RedirectToAction("Solve");
    }
    [HttpGet]

    public IActionResult Timer()
    {
        TimeSpan timerDuration = TimeSpan.FromMinutes(5); // Replace with your desired timer duration
        return View(timerDuration);
    }


}
