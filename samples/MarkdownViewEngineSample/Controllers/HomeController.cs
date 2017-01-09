using Microsoft.AspNetCore.Mvc;

namespace MarkdownViewEngineSample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();

        [HttpGet("/about")]
        public IActionResult About() => View();

        [HttpGet("/contact")]
        public IActionResult Contact() => View();
    }
}
