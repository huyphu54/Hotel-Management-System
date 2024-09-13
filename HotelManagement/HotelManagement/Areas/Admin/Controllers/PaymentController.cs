using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class PaymentController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("Payment")]
        public IActionResult Payment(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listHoaDon = db.HoaDons.Include(nv => nv.MaPhieuThueNavigation).AsNoTracking().OrderBy(x => x.MaHoaDon);
            return View(listHoaDon);
        }

        [Route("CreateBill")]
        [HttpGet]
        public IActionResult CreateBill(int? maPhieuThue)
        {
            var datPhong = db.DatPhongs.Include(dp => dp.PhongDichVus).ThenInclude(pdv => pdv.MaDichVuNavigation)
                .Include(p=>p.MaPhongNavigation).ThenInclude(lp=>lp.MaLpNavigation)
                .FirstOrDefault(dp => dp.MaPhieuThue == maPhieuThue);
            if (datPhong == null)
            {
                return NotFound();
            }
            double soNgayGio = 0;
            double tongTienDichVu = datPhong.PhongDichVus.Sum(dv =>dv.SoLuong.GetValueOrDefault() * (dv.MaDichVuNavigation?.Gia ?? 0));
           

            if (datPhong.NgayNhan.HasValue && datPhong.NgayTra.HasValue)
            {
                var ngayGioNhan = datPhong.GioNhan.HasValue ? datPhong.NgayNhan.Value.ToDateTime(datPhong.GioNhan.Value) : datPhong.NgayNhan.Value.ToDateTime(TimeOnly.MinValue);
                var ngayGioTra = datPhong.GioTra.HasValue ? datPhong.NgayTra.Value.ToDateTime(datPhong.GioTra.Value) : datPhong.NgayTra.Value.ToDateTime(TimeOnly.MaxValue);

                soNgayGio = (ngayGioTra - ngayGioNhan).TotalDays;
                
            }
            var ngayLapPhieu = datPhong.NgayTra;
            double tienPhong =datPhong.MaPhongNavigation.MaLpNavigation.Gia.Value * soNgayGio;
            double tienCoc = datPhong.MaPhongNavigation.MaLpNavigation.SoTienCoc.Value * soNgayGio;
            double tongTienTamTinh = 0;
            if (datPhong.MaTinhTrangDat==4)
            {
                tongTienTamTinh = tienCoc + tongTienDichVu;
            }
            else 
            {
                tongTienTamTinh = tienPhong + tongTienDichVu;
            }
            if(datPhong.MaTinhTrangDat == 1 )
            {
                ModelState.AddModelError("", "Khách Hàng Chưa Nhận Phòng!");
            }
            else if ( datPhong.MaTinhTrangDat == 2)
            {
                ModelState.AddModelError("", "Khách Hàng Chưa Trả Phòng!");
            }
            double phuThu = 0;

            HoaDon hoaDon = new HoaDon()
            {
                MaPhieuThue = maPhieuThue,
                NgayLapPhieu =ngayLapPhieu,
                TienDichVu = Math.Round(tongTienDichVu),
                SoTienCoc = Math.Round(tienCoc), 
                PhuThu = phuThu,
                TongTienTamTinh = Math.Round(tongTienTamTinh),
                TongTienThu = Math.Round(tongTienTamTinh +phuThu)
            };
            return View(hoaDon);
        }
        [Route("CreateBill")]
        [HttpPost]
        public IActionResult CreateBill(HoaDon hoaDon)
        {
            bool checkHoaDon = db.HoaDons.Any(pt => pt.MaPhieuThue == hoaDon.MaPhieuThue);
            if (checkHoaDon)
            {
                TempData["Message"] = "Phiếu thuê này đã được thanh toán";
                return View(hoaDon);
            }
            if (ModelState.IsValid)
            {
                db.HoaDons.Add(hoaDon);
                db.SaveChanges();
                return RedirectToAction("Payment", new { id = hoaDon.MaHoaDon });
            }

            return View(hoaDon);
        }
        [Route("PaymentDetail")]
        public IActionResult PaymentDetail(int? maHoaDon)
        {
            var hoaDon = db.HoaDons.Include(hd=>hd.MaPhieuThueNavigation).ThenInclude(dp=>dp.MaKhNavigation)
                .Include(hd => hd.MaPhieuThueNavigation).ThenInclude(dp => dp.MaNvNavigation)
                .Include(hd1 => hd1.MaPhieuThueNavigation).ThenInclude(dp => dp.PhongDichVus).ThenInclude(dv=>dv.MaDichVuNavigation)
                .Include(hd2 => hd2.MaPhieuThueNavigation).ThenInclude(dp => dp.MaPhongNavigation)
                .FirstOrDefault(hd=>hd.MaHoaDon == maHoaDon);
            return View(hoaDon);
        }


    }


}
