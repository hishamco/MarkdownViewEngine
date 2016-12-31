using Microsoft.AspNetCore.Mvc;

namespace MarkdownViewEngineSample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();
    }
}
