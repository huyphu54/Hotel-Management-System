using HotelManagement.Filters;
using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Authentication]
    [Area("Admin")]
  
    public class GroupRoomRentalController : Controller
    {
        QlksContext db = new QlksContext();
    
        [Route("Admin/GroupRoomRental")]
        public IActionResult GroupRoomRental(int? page, string searchDoan)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var datPhongDoan = db.DatPhongDoans
                .Include(dp => dp.MaKhNavigation)
                .Include(dp => dp.MaTinhTrangDatNavigation)
                .Include(dp => dp.MaNvNavigation)
                .Include(d => d.DatPhongDoanPhongs)
                .ThenInclude(dpd => dpd.MaPhongNavigation)
                .AsNoTracking().OrderBy(x => x.NgayDat);
            if (!string.IsNullOrEmpty(searchDoan))
            {
                datPhongDoan = datPhongDoan.Where(pd => pd.TenDoan.Contains(searchDoan))
                    .OrderBy(x => x.NgayDat);
            }
            ViewBag.SearchDoan = searchDoan;
            PagedList<DatPhongDoan> lst = new PagedList<DatPhongDoan>(datPhongDoan, pageNumber, pageSize);
            return View(lst);
        }

        [Route("Admin/AddGroupRental")]
        [HttpGet]
        public IActionResult AddGroupRental()
        {
            var customers = db.KhachHangs
             .Select(c => new { c.MaKh, c.HoTenKh, c.Cccd })
             .ToList();
            var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
            var receptionStaff = db.NhanViens.Where(nv => nv.MaPb == 2).Select(nv => new { nv.MaNv, nv.HoTenNv }).ToList();
            ViewBag.KhachHang = customers;
            ViewBag.NhanVien = new SelectList(receptionStaff, "MaNv", "HoTenNv");
            ViewBag.TinhTrang = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
            ViewBag.Phong = new MultiSelectList(availableRooms, "MaPhong", "SoPhong"); 

            return View();
        }
        [Route("Admin/AddGroupRental")]
        [HttpPost]
        public IActionResult AddGroupRental(DatPhongDoan datPhongDoan, List<int> selectedPhongIds)
        {
            
            var ngayHomNay = DateOnly.FromDateTime(DateTime.Now);
            var customers = db.KhachHangs
             .Select(c => new { c.MaKh, c.HoTenKh, c.Cccd })
             .ToList();
            var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
            var receptionStaff = db.NhanViens.Where(nv => nv.MaPb == 2).Select(nv => new { nv.MaNv, nv.HoTenNv }).ToList();

            bool checkDoan = db.DatPhongDoans.Any(d => d.TenDoan == datPhongDoan.TenDoan);
            if (checkDoan)
            {
                TempData["Message"] = "Đoàn này đã có trong hệ thống";
                ViewBag.KhachHang = customers;
                ViewBag.NhanVien = new SelectList(receptionStaff, "MaNv", "HoTenNv");
                ViewBag.TinhTrang = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
                ViewBag.Phong = new MultiSelectList(availableRooms, "MaPhong", "SoPhong");
                return View(datPhongDoan);
            }
            if (datPhongDoan.NgayDat >= datPhongDoan.NgayTra)
            {
                TempData["Message"]="Ngày nhận phải trước ngày trả.";
                ViewBag.KhachHang = customers;
                ViewBag.NhanVien = new SelectList(receptionStaff, "MaNv", "HoTenNv");
                ViewBag.TinhTrang = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
                ViewBag.Phong = new MultiSelectList(availableRooms, "MaPhong", "SoPhong");
                return View(datPhongDoan);
            }
            if (datPhongDoan.NgayDat < ngayHomNay)
            {
                TempData["Message"] = "Ngày đặt không thể ở quá khứ";
                ViewBag.KhachHang = customers;
                ViewBag.NhanVien = new SelectList(receptionStaff, "MaNv", "HoTenNv");
                ViewBag.TinhTrang = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
                ViewBag.Phong = new MultiSelectList(availableRooms, "MaPhong", "SoPhong");
                return View(datPhongDoan);
            }
            if (ModelState.IsValid)
            {

                db.DatPhongDoans.Add(datPhongDoan);
                db.SaveChanges();

                foreach (var phongId in selectedPhongIds)
                {
                    var datPhongDoanPhong = new DatPhongDoanPhong
                    {
                        MaDoan = datPhongDoan.MaDoan,
                        MaPhong = phongId
                    };
                    db.DatPhongDoanPhongs.Add(datPhongDoanPhong);
                }


                db.SaveChanges();

                return RedirectToAction("GroupRoomRental", "GroupRoomRental");
            }
            
            ViewBag.KhachHang = customers;
            ViewBag.NhanVien = new SelectList(receptionStaff, "MaNv", "HoTenNv");
            ViewBag.TinhTrang = new SelectList(db.TinhTrangDats, "MaTinhTrangDat", "TenTinhTrangDat");
            ViewBag.Phong = new MultiSelectList(availableRooms, "MaPhong", "SoPhong");

            return View(datPhongDoan);
        }
        [Route("admin/EditGroupRental")]
        [HttpGet]
        public IActionResult EditGroupRental(int? maDoan)
        {
            
            ViewBag.MaPhong = new MultiSelectList(db.Phongs.ToList(), "MaPhong", "SoPhong");
            ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats.ToList(), "MaTinhTrangDat", "TenTinhTrangDat");

            var phieuDatDoan = db.DatPhongDoans.Include(d => d.DatPhongDoanPhongs).FirstOrDefault(d => d.MaDoan == maDoan);
            ViewBag.SelectedRoomIds = phieuDatDoan.DatPhongDoanPhongs.Select(r => r.MaPhong).ToList();
           
            return View(phieuDatDoan);
        }
        [Route("admin/EditGroupRental")]
        [HttpPost]
        public IActionResult EditGroupRental(DatPhongDoan datPhongDoan, List<int> selectedRoomIds)
        {
            var updateStatus = db.DatPhongDoans.Include(d => d.DatPhongDoanPhongs).FirstOrDefault(d => d.MaDoan == datPhongDoan.MaDoan);
            var availableRooms = db.Phongs.Where(p => p.MaTinhTrang == 2).Select(p => new { p.MaPhong, p.SoPhong }).ToList();
            if (updateStatus == null)
            {
                ModelState.AddModelError("", "Đặt phòng không tồn tại.");
                return View(datPhongDoan);
            }

            if (updateStatus.MaTinhTrangDat == 3)
            {
                TempData["Message"] = "Không thể thay đổi tình trạng do đoàn đã trả phòng!";
                return RedirectToAction("GroupRoomRental", "GroupRoomRental");

           
            }
            if (updateStatus.MaTinhTrangDat == 4)
            {
                TempData["Message"] = "Không thể thay đổi tình trạng do phòng đã hủy!";
                return RedirectToAction("GroupRoomRental", "GroupRoomRental");
            }

            updateStatus.DatPhongDoanPhongs.Clear();
            updateStatus.MaTinhTrangDat = datPhongDoan.MaTinhTrangDat;
           
            foreach (var roomId in selectedRoomIds)
                {
                    var room = new DatPhongDoanPhong { MaPhong = roomId, MaDoan = datPhongDoan.MaDoan };
                    updateStatus.DatPhongDoanPhongs.Add(room); 
                    var phong = db.Phongs.FirstOrDefault(p => p.MaPhong == roomId);
                if (updateStatus.MaTinhTrangDat == 2)
                {
                    if (phong.MaTinhTrang == 1)
                    {
                        ModelState.AddModelError("", "Phòng này hiện đang có người thuê");
                        
                        ViewBag.MaPhong = new MultiSelectList(db.Phongs.ToList(), "MaPhong", "SoPhong");
                        ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats.ToList(), "MaTinhTrangDat", "TenTinhTrangDat");

                        return View(datPhongDoan);
                    }
                    else if (phong.MaTinhTrang == 3)
                    {
                        ModelState.AddModelError("", "Phòng này hiện đang sửa chữa");
                        ViewBag.MaPhong = new MultiSelectList(db.Phongs.ToList(), "MaPhong", "SoPhong");
                        ViewBag.MaTinhTrangDat = new SelectList(db.TinhTrangDats.ToList(), "MaTinhTrangDat", "TenTinhTrangDat");

                        return View(datPhongDoan);
                    }
                    phong.MaTinhTrang = 1;
                }
                else if (updateStatus.MaTinhTrangDat == 3)
                    {                   
                        if (phong != null)
                        {
                            phong.MaTinhTrang = 2; 
                            db.Entry(phong).State = EntityState.Modified;
                        }
                    }
               
                }
            
            db.Entry(updateStatus).State = EntityState.Modified;            
            db.SaveChanges();
            return RedirectToAction("GroupRoomRental", "GroupRoomRental");
           
        }
        [HttpGet("admin/addgroupservice")]
        public IActionResult AddGroupService(int? maDoan)
        {
            var doanDatPhong = db.DatPhongDoans
                             .Include(dp => dp.DoanDichVus)
                             .FirstOrDefault(dp => dp.MaDoan == maDoan);

            var dichVuDoan = db.DichVus.Where(dv => dv.TinhTrang == "Còn Hàng").Select(dv => new { dv.MaDichVu, dv.TenDichVu }).ToList(); 
            if (doanDatPhong == null)
            {
                return NotFound();
            }

            ViewBag.MaDichVu = new SelectList(dichVuDoan, "MaDichVu", "TenDichVu");

            return View(new DoanDichVu { MaDoan = maDoan.Value });
        }

        [HttpPost("admin/addgroupservice")]
        public IActionResult AddGroupService(DoanDichVu model, string action)
        {
            var existingRecord = db.DoanDichVus
                                  .AsNoTracking()
                                  .FirstOrDefault(ddv => ddv.MaDoan == model.MaDoan && ddv.MaDichVu == model.MaDichVu);
            var dichVuDoan = db.DichVus.Where(dv => dv.TinhTrang == "Còn Hàng").Select(dv => new { dv.MaDichVu, dv.TenDichVu }).ToList(); 
            if (existingRecord != null)
            {
                existingRecord.SoLuong += model.SoLuong.GetValueOrDefault();
                db.Entry(existingRecord).State = EntityState.Modified;
            }


            var groupbooking = db.DatPhongDoans.FirstOrDefault(d => d.MaDoan == model.MaDoan);
            if (groupbooking.MaTinhTrangDat == 1 || groupbooking.MaTinhTrangDat == 2)
            {
                if (action == "save")
                {
                    db.DoanDichVus.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("AddGroupService", new { maDoan = model.MaDoan });
                }
                else if (action == "exit")
                {
                    return RedirectToAction("Details", new { maDoan = model.MaDoan });
                }
            }
            ModelState.AddModelError("", "Không thể thêm dịch vụ!");
            ViewBag.MaDichVu = new SelectList(dichVuDoan, "MaDichVu", "TenDichVu");
            return View(model);
        }
        [HttpGet("admin/GroupDetail")]
        public IActionResult GroupDetail(int? maDoan)
        {
            var doanDatPhong = db.DatPhongDoans
                             .Include(dp => dp.DoanDichVus).ThenInclude(pd => pd.MaDichVuNavigation)
                             .Include(dp => dp.MaKhNavigation)
                             .Include(dp => dp.MaNvNavigation)
                             .Include(dp => dp.DatPhongDoanPhongs).ThenInclude(p => p.MaPhongNavigation).ThenInclude(lp=>lp.MaLpNavigation)
                             .Include(dp => dp.MaTinhTrangDatNavigation)
                             .FirstOrDefault(dp => dp.MaDoan == maDoan);

            if (doanDatPhong == null)
            {
                return NotFound();
            }

            return View(doanDatPhong);
        }
    }
}
