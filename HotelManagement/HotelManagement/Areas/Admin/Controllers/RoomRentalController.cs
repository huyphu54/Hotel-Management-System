using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using X.PagedList;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]

    public class RoomRentalController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("")]
        [Route("Roomrental")]
        public IActionResult RoomRental(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listPhieuDat = db.DatPhongs.Include(p => p.MaKhNavigation).Include(p => p.MaNvNavigation).Include(p => p.MaPhongNavigation).Include(p => p.MaLoaiHinhDatNavigation).Include(p => p.MaTinhTrangDatNavigation).AsNoTracking().OrderBy(x => x.MaPhieuThue).AsQueryable();
            PagedList <DatPhong> lstPhieuDat = new PagedList<DatPhong>(listPhieuDat, pageNumber, pageSize);
            return View(lstPhieuDat);
        }

        [Route("booking")]
        [HttpGet]

            public IActionResult Booking(int? roomId)
            {
                DatPhong booking = new DatPhong();
                if (roomId.HasValue)
                {
                    booking.MaPhong = roomId.Value;
                }
              
                ViewBag.MaKh = new SelectList(db.KhachHangs, "MaKh", "HoTenKh");
                ViewBag.MaNv = new SelectList(db.NhanViens, "MaNv", "HoTenNv");
                ViewBag.MaLoaiHinhDat = new SelectList(db.LoaiHinhDats, "MaLoaiHinhDat", "TenLoaiHinhDat");
                var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
                ViewBag.MaPhong = new SelectList(availableRooms, "MaPhong", "SoPhong");
                ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
           
            return View(booking);
           }
        
        [Route("booking")]
        [HttpPost]
        public IActionResult Booking(DatPhong book)
        {
            db.DatPhongs.Add(book);
            var room = db.Phongs.FirstOrDefault(p => p.MaPhong == book.MaPhong);

            if (room.MaTinhTrang == 2)
            {
                if (book.MaTinhTrangDat != 1)
                {
                    room.MaTinhTrang = 1;
                    db.Entry(room).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Room", "Room");
                }

                db.SaveChanges();
                return RedirectToAction("Room", "Room");
            }


            ModelState.AddModelError("", "Không thể dùng phòng này");
            return View(book);

        }
   
        [HttpGet("addservice")]
        public IActionResult AddService(int? maPhieuThue)
        {
            var datPhong = db.DatPhongs
                             .Include(dp => dp.PhongDichVus)
                             .FirstOrDefault(dp => dp.MaPhieuThue == maPhieuThue);

            
            if (datPhong == null)
            {
                return NotFound();
            }

            ViewBag.MaDichVu = new SelectList(db.DichVus, "MaDichVu", "TenDichVu");

            return View(new PhongDichVu { MaPhieuThue = maPhieuThue.Value } );
        }
        [HttpPost("addservice")]
        public IActionResult AddService(PhongDichVu model, string action)
        {
            var existingRecord = db.PhongDichVus
                                  .AsNoTracking()
                                  .FirstOrDefault(pdv => pdv.MaPhieuThue == model.MaPhieuThue && pdv.MaDichVu == model.MaDichVu);

            if (existingRecord != null)
            {
                // Nếu đã tồn tại, cập nhật số lượng
                existingRecord.SoLuong += model.SoLuong.GetValueOrDefault();
                db.Entry(existingRecord).State = EntityState.Modified;
            }


            var roombooking = db.DatPhongs.FirstOrDefault(p => p.MaPhieuThue == model.MaPhieuThue);
            if (roombooking.MaTinhTrangDat ==1 || roombooking.MaTinhTrangDat==2)
            {
                if (action == "save")
                {
                    db.PhongDichVus.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("AddService", new { maPhieuThue = model.MaPhieuThue });
                }
                else if (action == "exit")
                {
                    // Chuyển hướng sang trang khác (VD: chi tiết phiếu thuê phòng)
                    return RedirectToAction("Details", new { maPhieuThue = model.MaPhieuThue });

                }
            }



            ModelState.AddModelError("", "Không thể dùng phòng này");
            ViewBag.MaDichVu = new SelectList(db.DichVus, "MaDichVu", "TenDichVu");
            return View(model);
        }
        [HttpGet("details")]
        public IActionResult Details(int? maPhieuThue)
        {
            var datPhong = db.DatPhongs
                             .Include(dp => dp.PhongDichVus)
                             .ThenInclude(pd => pd.MaDichVuNavigation).Include(dp => dp.MaKhNavigation).Include(dp => dp.MaPhongNavigation).Include(dp => dp.MaTinhTrangDatNavigation)
                             .FirstOrDefault(dp => dp.MaPhieuThue == maPhieuThue);
           
            if (datPhong == null)
            {
                return NotFound();
            }

            return View(datPhong);
        }
    }
}
