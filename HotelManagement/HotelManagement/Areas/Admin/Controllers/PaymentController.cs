using DinkToPdf;
using DinkToPdf.Contracts;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class PaymentController : Controller
    {
        private readonly IConverter _converter;
       
        public PaymentController(IConverter converter)
        {
            _converter = converter;
            
        }

        QlksContext db = new QlksContext();
        [Route("Payment")]
        public IActionResult Payment(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listHoaDon = db.HoaDons.Include(nv => nv.MaPhieuThueNavigation).AsNoTracking().OrderBy(x => x.MaHoaDon);
            PagedList<HoaDon> lst = new PagedList<HoaDon>(listHoaDon, pageNumber, pageSize);
            return View(lst);
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
                ModelState.AddModelError("", "Phiếu Thuê Này Đã Được Thanh Toán");
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
        [Route("GeneratePdf")]
        [HttpGet]
        public IActionResult GeneratePdf(int? maHoaDon)
        {
            // Lấy dữ liệu hóa đơn từ cơ sở dữ liệu
            var hoaDon = db.HoaDons
                         .Include(hd => hd.MaPhieuThueNavigation)
                         .ThenInclude(dp => dp.MaKhNavigation)
                         .Include(hd => hd.MaPhieuThueNavigation)
                         .ThenInclude(dp => dp.MaNvNavigation)
                         .Include(hd => hd.MaPhieuThueNavigation)
                         .ThenInclude(dp => dp.PhongDichVus)
                         .ThenInclude(dv => dv.MaDichVuNavigation)
                         .Include(hd => hd.MaPhieuThueNavigation)
                         .ThenInclude(dp => dp.MaPhongNavigation)
                         .FirstOrDefault(hd => hd.MaHoaDon == maHoaDon);

            if (hoaDon == null)
            {
                return NotFound(); // Hoặc xử lý lỗi theo cách khác
            }

            var html = GenerateInvoiceHtml(hoaDon);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings() 
                {  Top = 10,
                    Bottom = 10,
                    Left = 10,
                    Right = 10 }
                },
                Objects = 
                {
                    new ObjectSettings()
                    {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", "HoaDon.pdf");
        }

        private string GenerateInvoiceHtml(HoaDon hoaDon)
        {
            var html = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .invoice {{ width: 100%; margin: 20px auto; }}
                            .invoice-header {{ text-align: center; }}
                            .invoice-header h1 {{ margin: 0; }}
                            .invoice-details {{ margin: 20px 0; }}
                            .invoice-details table {{ width: 100%; border-collapse: collapse; }}
                            .invoice-details th, .invoice-details td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                            .invoice-details th {{ background-color: #f2f2f2; }}
                            .total {{ font-weight: bold; }}
                        </style>
                    </head>
                <body>
                       <div class='invoice'>
                         <div class='invoice-header'>
                            <h1>HÓA ĐƠN THANH TOÁN</h1>
                            <p>Khách Sạn NP Luxury</p>
                            <p>Địa chỉ: 371 Nguyễn Kiệm, Phường 3, Quận Gò Vấp, Thành phố Hồ Chí Minh</p>
                            <p>Điện thoại: 0338790976</p>
                            <p>Email: npluxryht@gmaill.com</p>
                            
                        </div>
                        <div class='invoice-details'>
                    <h3>Thông Tin Hóa Đơn</h3>
                    <p>Mã Phiếu Thuê: {hoaDon.MaPhieuThue}</p>
                    <p>Nhân Viên Lập Phiếu: {hoaDon.MaPhieuThueNavigation.MaNvNavigation.HoTenNv}</p>
                    <p>Tên Khách Hàng: {hoaDon.MaPhieuThueNavigation.MaKhNavigation.HoTenKh}</p>
                    <p>Phòng Sử Dụng: {hoaDon.MaPhieuThueNavigation.MaPhongNavigation.SoPhong}</p>
                    <p>Ngày Lập: {hoaDon.NgayLapPhieu:dd/MM/yyyy}</p>

                    <h3>Dịch Vụ Đã Sử Dụng</h3>
                    <table>
                        <thead>
                            <tr>
                                <th>Mô tả</th>
                                <th>Số lượng</th>
                                <th>Đơn giá</th>
                                <th>Thành tiền</th>
                            </tr>
                        </thead>
                        <tbody>";

                        foreach (var dv in hoaDon.MaPhieuThueNavigation.PhongDichVus)
                        {
                            var thanhTien = dv.SoLuong * dv.MaDichVuNavigation.Gia;
                            html += $@"
                        <tr>
                            <td>{dv.MaDichVuNavigation.TenDichVu}</td>
                            <td>{dv.SoLuong}</td>
                            <td>{dv.MaDichVuNavigation.Gia:N0} VND</td>
                            <td>{thanhTien:N0} VND</td>
                        </tr>";
                        }

                        html += $@"
                                    </tbody>
                                </table>
                                <p class='total'>Tổng tiền dịch vụ: {hoaDon.TienDichVu:N0} VND</p>
                                <p class='total'>Số tiền cọc: {hoaDon.SoTienCoc:N0} VND</p>
                                <p class='total'>Phụ thu: {hoaDon.PhuThu:N0} VND</p>
                                <p class='total'>Tổng tiền thanh toán: {hoaDon.TongTienThu:N0} VND</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            return html;
        }
    }
}



