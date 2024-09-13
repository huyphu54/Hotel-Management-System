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
        //Danh sách phiếu thuê
        [Route("Roomrental")]
        public IActionResult RoomRental(int? page, string searchKhachHang, string searchLoaiHinhDat, string searchTinhTrang)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listPhieuDat = db.DatPhongs.Include(p => p.MaKhNavigation).Include(p => p.MaNvNavigation).Include(p => p.MaPhongNavigation).ThenInclude(lp => lp.MaLpNavigation)
                .Include(p => p.MaLoaiHinhDatNavigation).Include(p => p.MaTinhTrangDatNavigation).AsNoTracking().OrderBy(x => x.MaPhieuThue).AsQueryable();

            if (!string.IsNullOrEmpty(searchKhachHang))
            {
                listPhieuDat = listPhieuDat.Where(pd => pd.MaKhNavigation.HoTenKh.Contains(searchKhachHang))
                    .OrderBy(x => x.MaPhieuThue);
            }
            if (!string.IsNullOrEmpty(searchLoaiHinhDat))
            {
                listPhieuDat = listPhieuDat.Where(pd => pd.MaLoaiHinhDatNavigation.TenLoaiHinhDat.Contains(searchLoaiHinhDat))
                    .OrderBy(x => x.MaPhieuThue);
            }
            if (!string.IsNullOrEmpty(searchTinhTrang))
            {
                listPhieuDat = listPhieuDat.Where(pd => pd.MaTinhTrangDatNavigation.TenTinhTrangDat.Contains(searchTinhTrang))
                    .OrderBy(x => x.MaPhieuThue);
            }

            ViewBag.SearchKhachHang = searchKhachHang;
            ViewBag.SearchLoaiHinhDat = searchLoaiHinhDat;
            ViewBag.SearchTinhTrang = searchTinhTrang;
            PagedList<DatPhong> lstPhieuDat = new PagedList<DatPhong>(listPhieuDat, pageNumber, pageSize);
            return View(lstPhieuDat);
        }

        //Thay đổi chi tiết phiếu thuê
        [Route("Editrental")]
        [HttpGet]
        public IActionResult EditRental(int? maPhieuThue)
        {
            ViewBag.MaPhong = new SelectList(db.Phongs.ToList(), "MaPhong", "SoPhong");
            ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats.ToList(), "MaTinhTrangDat", "TenTinhTrangDat");
            var phieuThue = db.DatPhongs.Find(maPhieuThue);
            return View(phieuThue);

        }

        [Route("Editrental")]
        [HttpPost]
        public IActionResult EditRental(DatPhong datPhong)
        {
            var updateStatus = db.DatPhongs.Find(datPhong.MaPhieuThue);
            if (updateStatus.MaTinhTrangDat == 3)
            {
                ModelState.AddModelError("", "Không thể thay đổi tình trạng do phòng đã trả");
                return View(datPhong);
            }
            if(updateStatus.MaTinhTrangDat == 4)
            {
                ModelState.AddModelError("", "Không thể thay đổi tình trạng do phòng đã hủy");
                return View(datPhong);
            }
            updateStatus.MaTinhTrangDat = datPhong.MaTinhTrangDat;
            updateStatus.GioNhan = datPhong.GioNhan;
            updateStatus.GioTra = datPhong.GioTra;
            var phongUpdate = db.Phongs.FirstOrDefault(p => p.MaPhong == datPhong.MaPhong);
            if (phongUpdate == null)
            {
                return NotFound("Phòng không tồn tại hoặc không hợp lệ.");
            }


            if (updateStatus.MaTinhTrangDat == 2)
            {
                if (phongUpdate.MaTinhTrang == 1)
                {
                    ModelState.AddModelError("", "Phòng này hiện đang có người thuê");
                    ViewBag.MaPhong = new SelectList(db.Phongs.ToList(), "MaPhong", "SoPhong");
                    ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats.ToList(), "MaTinhTrangDat", "TenTinhTrangDat");
                    return View(datPhong);
                }
                phongUpdate.MaTinhTrang = 1;
            }

            else if (updateStatus.MaTinhTrangDat == 3)
            {
                phongUpdate.MaTinhTrang = 2;
            }

            

            db.Entry(updateStatus).State = EntityState.Modified;
            db.Entry(phongUpdate).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("RoomRental", "RoomRental");
        }

        //Thêm Phiếu Thuê mới
        [Route("booking")]
        [HttpGet]
        public IActionResult Booking(int? roomId)
        {
            DatPhong booking = new DatPhong();
            if (roomId.HasValue)
            {
                booking.MaPhong = roomId.Value;
            }
            var customers = db.KhachHangs
              .Select(c => new { c.MaKh, c.HoTenKh })
              .ToList();
            ViewBag.KhachHang = new SelectList(customers, "MaKh", "HoTenKh");
            ViewBag.MaNv = new SelectList(db.NhanViens, "MaNv", "HoTenNv");
            ViewBag.MaLoaiHinhDat = new SelectList(db.LoaiHinhDats, "MaLoaiHinhDat", "TenLoaiHinhDat");
            var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
            ViewBag.MaPhong = new SelectList(availableRooms, "MaPhong", "SoPhong");
            ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");

            return View(booking);
        }

        [Route("booking")]
        [HttpPost]
        public IActionResult Booking(DatPhong datPhong)
        {
            db.DatPhongs.Add(datPhong);
            var phong = db.Phongs.Include(dp => dp.MaLpNavigation).FirstOrDefault(p => p.MaPhong == datPhong.MaPhong);
            var loaiPhong = phong.MaLpNavigation;

            if (datPhong.SoNguoiO > loaiPhong.SoNguoiOtoiDa)
            {
                ModelState.AddModelError("", $"Số lượng người ở vượt quá giới hạn cho phép. Phòng này chỉ cho phép tối đa {loaiPhong.SoNguoiOtoiDa} người.");
                return View(datPhong);
            }

            if (phong.MaTinhTrang == 2)
            {
                if (datPhong.MaTinhTrangDat != 1)
                {
                    phong.MaTinhTrang = 1;
                    db.Entry(phong).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Room", "Room");
                }

                db.SaveChanges();
                return RedirectToAction("Room", "Room");
            }

            ModelState.AddModelError("", "Không thể dùng phòng này");
            ViewBag.MaKh = new SelectList(db.KhachHangs, "MaKh", "HoTenKh");
            ViewBag.MaNv = new SelectList(db.NhanViens, "MaNv", "HoTenNv");
            ViewBag.MaLoaiHinhDat = new SelectList(db.LoaiHinhDats, "MaLoaiHinhDat", "TenLoaiHinhDat");
            var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
            ViewBag.MaPhong = new SelectList(availableRooms, "MaPhong", "SoPhong");
            ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");

            return View(datPhong);
        }

        //Thêm Dịch Vụ
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

            return View(new PhongDichVu { MaPhieuThue = maPhieuThue.Value });
        }

        [HttpPost("addservice")]
        public IActionResult AddService(PhongDichVu model, string action)
        {
            var existingRecord = db.PhongDichVus
                                  .AsNoTracking()
                                  .FirstOrDefault(pdv => pdv.MaPhieuThue == model.MaPhieuThue && pdv.MaDichVu == model.MaDichVu);

            if (existingRecord != null)
            {
                existingRecord.SoLuong += model.SoLuong.GetValueOrDefault();
                db.Entry(existingRecord).State = EntityState.Modified;
            }


            var roombooking = db.DatPhongs.FirstOrDefault(p => p.MaPhieuThue == model.MaPhieuThue);
            if (roombooking.MaTinhTrangDat == 1 || roombooking.MaTinhTrangDat == 2)
            {
                if (action == "save")
                {
                    db.PhongDichVus.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("AddService", new { maPhieuThue = model.MaPhieuThue });
                }
                else if (action == "exit")
                {
                    return RedirectToAction("Details", new { maPhieuThue = model.MaPhieuThue });

                }
            }



            ModelState.AddModelError("", "Không thể dùng phòng này");
            ViewBag.MaDichVu = new SelectList(db.DichVus, "MaDichVu", "TenDichVu");
            return View(model);
        }

        //Chi Tiết Phiếu Thuê
        [HttpGet("details")]
        public IActionResult Details(int? maPhieuThue)
        {
            var datPhong = db.DatPhongs
                             .Include(dp => dp.PhongDichVus).ThenInclude(pd => pd.MaDichVuNavigation)
                             .Include(dp => dp.MaKhNavigation)
                             .Include(dp => dp.MaNvNavigation)
                             .Include(dp => dp.MaPhongNavigation).ThenInclude(p => p.MaLpNavigation)
                             .Include(dp => dp.MaTinhTrangDatNavigation)
                             .FirstOrDefault(dp => dp.MaPhieuThue == maPhieuThue);

            if (datPhong == null)
            {
                return NotFound();
            }

            return View(datPhong);
        }

    }
}
    