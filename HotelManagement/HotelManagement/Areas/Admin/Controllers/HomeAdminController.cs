using HotelManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]


    public class HomeAdminController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("")]
        [Route("index")]
        
        public IActionResult Index()
        {
           
            return View();  
        }




        [Route("customer")]
        public IActionResult Customer(int? page, string searchString, string year)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Lấy danh sách khách hàng từ cơ sở dữ liệu
            var listKhachHang = db.KhachHangs.AsNoTracking().OrderBy(x => x.MaKh).AsQueryable();

            // Kiểm tra nếu có chuỗi tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                listKhachHang = listKhachHang.Where(kh => kh.HoTenKh.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(year))
            {
                listKhachHang = listKhachHang.Where(kh => kh.NamSinh.Contains(year));
            }


            // Sử dụng PagedList cho việc phân trang
            PagedList<KhachHang> lst = new PagedList<KhachHang>(listKhachHang, pageNumber, pageSize);

            // Truyền giá trị tìm kiếm vào ViewBag để hiển thị lại trên form tìm kiếm
            ViewBag.SearchString = searchString;
            ViewBag.Year = year;
            return View(lst);
        }



        [Route("addcustomer")]
        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }
        [Route("addcustomer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCustomer(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Customer");
            }
            return View(khachHang);
        }


    }
}
