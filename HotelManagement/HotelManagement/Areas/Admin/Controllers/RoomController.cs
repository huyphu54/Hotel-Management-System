using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class RoomController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("")]
        [Route("room")]
        public IActionResult Room(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listPhong = db.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTangNavigation).Include(p => p.MaTinhTrangNavigation).AsNoTracking().OrderBy(x => x.MaPhong);
            PagedList<Phong> lstPhong = new PagedList<Phong>(listPhong, pageNumber, pageSize);
            return View(lstPhong);
        }


        [Route("editroom")]
        [HttpGet]
        public IActionResult EditRoom(int? maPhong)
        {

            ViewBag.MaTang = new SelectList(db.Tangs.ToList(), "MaTang", "TenTang");
            ViewBag.MaLp = new SelectList(db.LoaiPhongs.ToList(), "MaLp", "TenLoaiPhong");
            ViewBag.MaTinhTrang = new SelectList(db.TinhTrangPhongs.ToList(), "MaTinhTrang", "TenTinhTrang");
            var phong = db.Phongs.Find(maPhong);
            return View(phong);
        }

        [Route("editroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRoom(Phong phong, DatPhong datPhong)
        {
            var phongToUpdate = db.Phongs.Find(phong.MaPhong);

            if (phongToUpdate == null)
            {
                return NotFound();
            }

            phongToUpdate.SoPhong = phong.SoPhong;
            phongToUpdate.MaTang = phong.MaTang;
            phongToUpdate.MaLp = phong.MaLp;
            phongToUpdate.MaTinhTrang = phong.MaTinhTrang;

            db.Entry(phongToUpdate).State = EntityState.Modified;

            // Cập nhật trạng thái phiếu đặt phòng nếu thay đổi trạng thái phòng
            if (phongToUpdate.MaTinhTrang == 2)
            {
                // Tìm phiếu đặt phòng tương ứng (nếu có)
                var datPhongUpdate = db.DatPhongs.FirstOrDefault(dp => dp.MaPhong == phong.MaPhong && dp.MaTinhTrangDat == 2);

                if (datPhongUpdate != null)
                {
                    datPhongUpdate.MaTinhTrangDat = 3;  // Cập nhật tình trạng đặt phòng
                    db.Entry(datPhongUpdate).State = EntityState.Modified;
                }
            }

            var id = db.SaveChanges();
            if (id > 0)
            {
                return RedirectToAction("Room");
            }
            else
            {
                ModelState.AddModelError("", "Thay đổi dữ liệu không thành công");
                return View(phong);
            }
        }


        [Route("roomtype")]
        public IActionResult RoomType()
        {
            var listLoaiPhong = db.LoaiPhongs.ToList();
            return View(listLoaiPhong);
        }

        [Route("addroomtype")]
        [HttpGet]
        public IActionResult AddRoomType()
        {
            return View();
        }
        [Route("addroomtype")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRoomType(LoaiPhong loaiphong)
        {
            if (ModelState.IsValid)
            {
                db.LoaiPhongs.Add(loaiphong);
                db.SaveChanges();
                return RedirectToAction("roomtype");
            }
            return View(loaiphong);
        }

        [Route("editroomtype")]
        [HttpGet]
        public IActionResult EditRoomType(int? maLoaiPhong)
        {
            var loaiPhong = db.LoaiPhongs.Find(maLoaiPhong);
            return View(loaiPhong);
        }
        [Route("editroomtype")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRoomType(LoaiPhong loaiphong)
        {
            if (ModelState.IsValid)
            {
                db.Update(loaiphong);
                db.SaveChanges();
                return RedirectToAction("RoomType");
            }
            return View(loaiphong);
        }

       
     
    }

}
