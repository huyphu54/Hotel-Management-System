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
    [Route("admin")]
    public class LoginController : Controller
    {
        private readonly QlksContext _db;

        public LoginController(QlksContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string taiKhoan, string matKhau, bool rememberMe, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Xác thực người dùng
                var user = await _db.Users.SingleOrDefaultAsync(u => u.TaiKhoan == taiKhoan && u.MatKhau == matKhau);
                if (user != null)
                {
                    // Tạo claims cho người dùng
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.TaiKhoan),
                        // Thêm các claim khác nếu cần
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe
                    };

                    // Đăng nhập người dùng
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Chuyển hướng về URL gốc hoặc trang mặc định
                    return Redirect(returnUrl ?? "/admin/index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        
    }
}
