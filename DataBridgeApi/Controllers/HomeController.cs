using Microsoft.AspNetCore.Mvc;

namespace DataBridgeApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Witaj na stronie!");
        }
    }
}