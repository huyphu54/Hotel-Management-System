using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
