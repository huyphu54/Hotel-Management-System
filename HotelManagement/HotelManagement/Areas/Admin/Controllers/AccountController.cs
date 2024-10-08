using HotelManagement.Filters;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Authentication]
    [Area("admin")]
    public class AccountController : Controller
    {

        QlksContext db = new QlksContext();
        [Authorization]
        [Route("admin/account")]
        public IActionResult Account()
        {
            var listAccount = db.Users.Include(u => u.MaNvNavigation).ToList();
            return View(listAccount);
        }
        [Authorization]
        [Route("admin/createaccount")]
        [HttpGet]
        public IActionResult CreateAccount()
        {
            ViewBag.MaNv = new SelectList(db.NhanViens,"MaNv","HoTenNv").ToList();
            return View();
        }
        [Route("admin/createaccount")]
        [HttpPost]
        public IActionResult CreateAccount(User user)
        {
            bool checkAccount = db.Users.Any(u => u.TaiKhoan == user.TaiKhoan);
            if (checkAccount)
            {
                ModelState.AddModelError("TaiKhoan", "Tên Đăng Nhập Đã Tồn Tại Trong Hệ Thống.");
                ViewBag.MaNv = new SelectList(db.NhanViens, "MaNv", "HoTenNv").ToList();
                return View(user);
            } 
         
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Account");
           
        }
    }
}
