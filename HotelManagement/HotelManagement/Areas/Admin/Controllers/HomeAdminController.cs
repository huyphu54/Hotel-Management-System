using HotelManagement.Filters;
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

        //Trang chủ
        
        [Route("")]
        [Authentication]
        [Route("index")]
        public IActionResult Index()
        {
           
            return View();  
        }

        //Danh Sách khách Hàng
        [Route("customer")]
        public IActionResult Customer(int? page, string searchString, string year)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var listKhachHang = db.KhachHangs.AsNoTracking().OrderBy(x => x.MaKh).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                listKhachHang = listKhachHang.Where(kh => kh.HoTenKh.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(year))
            {
                listKhachHang = listKhachHang.Where(kh => kh.NamSinh.Contains(year));
            }
            PagedList<KhachHang> lst = new PagedList<KhachHang>(listKhachHang, pageNumber, pageSize);
            ViewBag.SearchString = searchString;
            ViewBag.Year = year;
            return View(lst);
        }

        //Thêm Khách hàng mới
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
           bool checkCCCD = db.KhachHangs.Any(kh => kh.Cccd == khachHang.Cccd);
            if (checkCCCD)
            {              
                ModelState.AddModelError("CCCD", "Khách hàng này đã tồn tại trong hệ thống.");
                return View(khachHang); 
            }
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
