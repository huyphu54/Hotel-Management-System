using HotelManagement.Filters;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomCancelledController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("admin/roomcancelled")]
        public IActionResult RoomCancelled(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listPhongHuy = db.PhongHuys.Include(dp=>dp.MaPhieuThueNavigation).ThenInclude(kh=>kh.MaKhNavigation).ToList();
            PagedList<PhongHuy> lstPhongHuy = new PagedList<PhongHuy>(listPhongHuy, pageNumber, pageSize);
            return View(lstPhongHuy);
        }
        [Route("admin/editStatus")]
        [HttpGet]
        public IActionResult EditStatus(int? maPhongHuy)
        {
            var phongHuy = db.PhongHuys.Find(maPhongHuy);
            return View(phongHuy);

        }
        [Route("admin/editStatus")]
        [HttpPost]
        public IActionResult EditStatus(PhongHuy phongHuy)
        {
           
            var phongHuyUpdate = db.PhongHuys.Find(phongHuy.MaPhongHuy);

            if (phongHuyUpdate == null)
            {
                return NotFound(); 
            }
            if (phongHuyUpdate.TinhTrang != "Chưa Cập Nhật")
            {
                ModelState.AddModelError("", "Không thể cập nhật lý do");
                return View(phongHuy);
            }
            phongHuyUpdate.MaPhieuThue = phongHuy.MaPhieuThue;
            phongHuyUpdate.LyDo = phongHuy.LyDo;
            phongHuyUpdate.TinhTrang = phongHuy.TinhTrang;
           
            db.Entry(phongHuyUpdate).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RoomCancelled");
        }

    }
}
