using Microsoft.AspNetCore.Mvc;

namespace TangyWeb_API.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
