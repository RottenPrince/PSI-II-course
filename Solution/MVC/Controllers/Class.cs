using Microsoft.AspNetCore.Mvc;

public class TimerController : Controller
{
    private readonly YourDbContext _context;

    public TimerController(YourDbContext context)
    {
        _context = context;
    }

}
