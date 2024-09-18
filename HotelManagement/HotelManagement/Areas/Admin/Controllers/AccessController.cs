using Microsoft.AspNetCore.Mvc;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/login")]
    public class AccessController : Controller
    {
        QlksContext db = new QlksContext();
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("TaiKhoan") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            if (HttpContext.Session.GetString("TaiKhoan") == null)
            {
                var u = db.Users.Include(nv=>nv.MaNvNavigation)
              .Where(x => x.TaiKhoan == user.TaiKhoan && x.MatKhau == user.MatKhau)
              .FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("TaiKhoan", u.TaiKhoan.ToString());
                    HttpContext.Session.SetString("HoTenNv", u.MaNvNavigation.HoTenNv.ToString());
                    HttpContext.Session.SetInt32("MaNv", u.MaNv);
                    if (u.MaNvNavigation.Avatar != null)
                    {
                        var avatarBase64 = Convert.ToBase64String(u.MaNvNavigation.Avatar);
                        HttpContext.Session.SetString("Avatar", $"data:image/jpg;base64,{avatarBase64}"); // Thay đổi 'image/png' nếu cần
                    }
                    else
                    {
                        HttpContext.Session.SetString("Avatar", "img/boy.png"); // Hình mặc định nếu không có avatar
                    }
                    return RedirectToAction("Index", "HomeAdmin");
                }
            }
            return View();

        }
        [HttpPost]
        [Route("admin/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ thông tin phiên
            TempData["LogoutMessage"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Login", "Access");
        }

    }
}
