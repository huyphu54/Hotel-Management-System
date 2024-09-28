using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Controllers
{
    public class RoomController : Controller
    {
        [Route("room")]
        public IActionResult Room()
        {
            return View();
        }
    }
}
