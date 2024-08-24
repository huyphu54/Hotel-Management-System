using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
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
        [Route("room")]
        public IActionResult Room(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listPhong = db.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTangNavigation).AsNoTracking().OrderBy(x => x.MaPhong);
            PagedList<Phong> lstPhong = new PagedList<Phong>(listPhong, pageNumber, pageSize);
            return View(lstPhong);
        }
        [Route("customer")]
        public IActionResult Customer(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var listKhachHang = db.KhachHangs.AsNoTracking().OrderBy(x => x.MaKh);
            PagedList<KhachHang> lst = new PagedList<KhachHang>(listKhachHang, pageNumber, pageSize);
            return View(lst);
        }
        [Route("roomtype")]
        public IActionResult RoomType()
        {
            var listLoaiPhong = db.LoaiPhongs.ToList();
            return View(listLoaiPhong);
        }

    }
}
