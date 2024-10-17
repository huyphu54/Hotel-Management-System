using HotelManagement.Filters;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]

    public class RoomController : Controller
    {
        QlksContext db = new QlksContext();
        //Danh sách phòng
        [Route("admin/room")]
        public IActionResult Room(int? page, string searchTinhTrang, string searchLoaiPhong, string searchTang, DateOnly? searchDate)
        {
            int pageSize = 20;
            int pageNumber = page ?? 1;
            var listPhong = db.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTangNavigation).Include(p => p.MaTinhTrangNavigation).AsNoTracking().OrderBy(x => x.MaPhong);
            if (!string.IsNullOrEmpty(searchTinhTrang))
            {

                listPhong = listPhong.Where(p => p.MaTinhTrangNavigation.TenTinhTrang.Contains(searchTinhTrang))
                    .OrderBy(x => x.MaPhong);
            }
            if (!string.IsNullOrEmpty(searchLoaiPhong))
            {

                listPhong = listPhong.Where(p => p.MaLpNavigation.TenLoaiPhong.Contains(searchLoaiPhong))
                    .OrderBy(x => x.MaPhong);
            }
            if (!string.IsNullOrEmpty(searchTang))
            {

                listPhong = listPhong.Where(p => p.MaTangNavigation.TenTang.Contains(searchTang))
                    .OrderBy(x => x.MaPhong);
            }
            if (searchDate.HasValue)
            {
                // Giả sử DatPhong có thuộc tính NgayDat kiểu DateOnly  
                listPhong = (IOrderedQueryable<Phong>)listPhong.Where(p =>
                    !db.DatPhongs.Any(dp => dp.MaPhong == p.MaPhong && dp.NgayNhan == searchDate.Value )
                    && !db.DatPhongDoanPhongs.Any(dpd => dpd.MaPhong == p.MaPhong && dpd.MaDoanNavigation.NgayDat == searchDate.Value ) && p.MaTinhTrang == 2);
            }
            PagedList<Phong> lstPhong = new PagedList<Phong>(listPhong, pageNumber, pageSize);
            ViewBag.SearchTinhTrang = searchTinhTrang;
            ViewBag.SearchLoaiPhong= searchLoaiPhong;
            ViewBag.SearchTang = searchTang;
            ViewBag.SearchDate = searchDate;
            return View(lstPhong);

        }
        //Thêm Phòng Mới
        [Authorization]
        [Route("admin/addroom")]
        [HttpGet]
        public IActionResult AddRoom()
        {
            ViewBag.MaTang = new SelectList(db.Tangs.ToList(), "MaTang", "TenTang");
            ViewBag.MaLp = new SelectList(db.LoaiPhongs.ToList(), "MaLp", "TenLoaiPhong"); 
            ViewBag.MaTinhTrang = new SelectList(db.TinhTrangPhongs.ToList(), "MaTinhTrang", "TenTinhTrang");
            return View();
        }

        [Route("admin/addroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRoom(Phong phong)
        {
            
            bool checkPhong = db.Phongs.Any(p => p.SoPhong == phong.SoPhong);
            if (checkPhong)
            {
                TempData["Message"] = "Phòng này đã tồn tại trong hệ thống!";
                ViewBag.MaTang = new SelectList(db.Tangs.ToList(), "MaTang", "TenTang");
                ViewBag.MaLp = new SelectList(db.LoaiPhongs.ToList(), "MaLp", "TenLoaiPhong");
                ViewBag.MaTinhTrang = new SelectList(db.TinhTrangPhongs.ToList(), "MaTinhTrang", "TenTinhTrang");
                return View(phong);
            }
            db.Phongs.Add(phong);
            db.SaveChanges();
            return RedirectToAction("room");

        }
        //Xóa Phòng
        [Authorization]
        [Route("admin/DeleteRoom")]
        [HttpGet]
        public IActionResult DeleteRoom(int? maPhong)
        {
            var phong = db.Phongs.Find(maPhong);
            if (phong == null)
            {
                return NotFound();
            }

            // Xử lý các đơn đặt phòng liên quan
            var datPhongs = db.DatPhongs.Where(dp => dp.MaPhong == maPhong).ToList();
            foreach (var datPhong in datPhongs)
            {
                TempData["Message"] = "Không thể xóa phòng này.";
                return RedirectToAction("Room");
            }

            db.Phongs.Remove(phong);
            db.SaveChanges();

            TempData["Message"] = "Đã xóa phòng.";
            return RedirectToAction("Room");
        }
        //Thay đổi chi tiết phòng

        [Route("admin/editroom")]
        [HttpGet]
        public IActionResult EditRoom(int? maPhong)
        {

            ViewBag.MaTang = new SelectList(db.Tangs.ToList(), "MaTang", "TenTang");
            ViewBag.MaLp = new SelectList(db.LoaiPhongs.ToList(), "MaLp", "TenLoaiPhong");
            ViewBag.MaTinhTrang = new SelectList(db.TinhTrangPhongs.ToList(), "MaTinhTrang", "TenTinhTrang");
            var phong = db.Phongs.Find(maPhong);
            return View(phong);
        }

        [Route("admin/editroom")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRoom(Phong phong, DatPhong datPhong)
        {
            var phongToUpdate = db.Phongs.Find(phong.MaPhong);

            if (phongToUpdate == null)
            {
                return NotFound();
            }          
            phongToUpdate.MaTinhTrang = phong.MaTinhTrang;
            db.Entry(phongToUpdate).State = EntityState.Modified;

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


        [Route("admin/roomdetails")]
        [HttpGet]
        public IActionResult RoomDetails(int? maPhong)
        {  
            var phong = db.Phongs.Include(lp=>lp.MaLpNavigation)
                .Include(t=>t.MaTangNavigation)
                .Include(tt=>tt.MaTinhTrangNavigation)
                .Include(dp=>dp.DatPhongs).ThenInclude(kh=>kh.MaKhNavigation)
                .Include(dpd => dpd.DatPhongDoanPhongs).ThenInclude(d => d.MaDoanNavigation)
                .FirstOrDefault(p => p.MaPhong == maPhong);
            return View(phong);
        }


        //--------------------------Loại Phòng----------------------------

        [Authorization]
        //Danh sách loại phòng 
        [Route("admin/roomtype")]
        public IActionResult RoomType()
        {
            var listLoaiPhong = db.LoaiPhongs.ToList();
            return View(listLoaiPhong);
        }
        [Authorization]
        //Thêm loại phòng mới
        [Route("admin/addroomtype")]
        [HttpGet]
        public IActionResult AddRoomType()
        {
            return View();
        }

        [Route("admin/addroomtype")]
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
        [Authorization]
        //Chỉnh sửa loại phòng
        [Route("admin/editroomtype")]
        [HttpGet]
        public IActionResult EditRoomType(int? maLoaiPhong)
        {
            var loaiPhong = db.LoaiPhongs.Find(maLoaiPhong);
            return View(loaiPhong);
        }

        [Route("admin/editroomtype")]
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
