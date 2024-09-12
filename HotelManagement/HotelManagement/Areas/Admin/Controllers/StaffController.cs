using HotelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    public class StaffController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("Staff")]
        [HttpGet]
        public IActionResult Staff(int? page, string searchNhanVien, string searchPhongBan)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listNhanVien = db.NhanViens.Include(nv => nv.MaPbNavigation).AsNoTracking().OrderBy(x => x.MaNv);
            if (!string.IsNullOrEmpty(searchNhanVien))
            {

                listNhanVien = listNhanVien.Where(nv => nv.HoTenNv.Contains(searchNhanVien))
                    .OrderBy(x => x.MaNv);
            }
            if (!string.IsNullOrEmpty(searchPhongBan))
            {

                listNhanVien = listNhanVien.Where(nv => nv.MaPbNavigation.TenPb.Contains(searchPhongBan))
                    .OrderBy(x => x.MaNv);
            }

            PagedList<NhanVien> lstNhanVien = new PagedList<NhanVien>(listNhanVien, pageNumber, pageSize);
            ViewBag.SearchNhanVien = searchNhanVien;
            ViewBag.SearchPhongBan = searchPhongBan;
            return View(lstNhanVien);
        }
        [Route("AddStaff")]
        [HttpGet]
        public IActionResult AddStaff()
        {
            ViewBag.MaPb = new SelectList(db.PhongBans, "MaPb", "TenPb");
            return View();
        }
        [Route("AddStaff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStaff(NhanVien nhanVien, IFormFile AvatarFile)
        {
            bool checkEmail = db.NhanViens.Any(nv => nv.Email == nhanVien.Email);
            if (checkEmail)
            {
                ModelState.AddModelError("EMAIL", "Nhân viên này đã tồn tại trong hệ thống.");
                ViewBag.MaPb = new SelectList(db.PhongBans, "MaPb", "TenPb");
                return View(nhanVien);
            }

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    AvatarFile.CopyTo(ms);
                    nhanVien.Avatar = ms.ToArray();
                }
            }
            db.NhanViens.Add(nhanVien);
            db.SaveChanges();
            TempData["Message"] = "Thêm Nhân Viên Thành Công!";
            return RedirectToAction("Staff");
        }
        [Route("EditStaff")]
        [HttpGet]
        public IActionResult EditStaff(int? maNhanVien)
        {
            ViewBag.MaPb = new SelectList(db.PhongBans, "MaPb", "TenPb");
            var nhanVien = db.NhanViens.Find(maNhanVien);
            return View(nhanVien);
        }
        [Route("EditStaff")]
        [HttpPost]
        public IActionResult EditStaff(NhanVien nhanVien, IFormFile? AvatarFile)
        {
            var nhanVienUpdate = db.NhanViens.Find(nhanVien.MaNv);
            if (nhanVienUpdate == null)
            {
                return NotFound();
            }
            nhanVienUpdate.MaPb = nhanVien.MaPb;
            nhanVienUpdate.Sdt = nhanVien.Sdt;
            nhanVienUpdate.Email = nhanVien.Email;
            nhanVienUpdate.ChucVu = nhanVien.ChucVu;
 
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    AvatarFile.CopyTo(ms);
                    nhanVienUpdate.Avatar = ms.ToArray(); 
                }
            }

            db.Entry(nhanVienUpdate).State = EntityState.Modified;
            var id = db.SaveChanges();
            if (id > 0)
            {
                TempData["Message"] = "Cập Nhật Thông Tin Thành Công!";
                return RedirectToAction("Staff");
            }
            else
            {
                ModelState.AddModelError("", "Thay đổi dữ liệu không thành công!");
                return View(nhanVien);
            }
        }
        [Route("DetailStaff")]
        [HttpGet]
        public IActionResult DetailStaff(int? maNhanVien)
        {

            var nhanVien = db.NhanViens.Include(nv => nv.MaPbNavigation).FirstOrDefault(nv=>nv.MaNv == maNhanVien);
            return View(nhanVien);
        }
        [Route("DeleteStaff")]
        [HttpGet]
        public IActionResult DeleteStaff(int? maNhanVien)
        {
            var nhanVien = db.NhanViens.Find(maNhanVien);
            if (nhanVien == null)
            {
                return NotFound();
            }
            int NhanVienQuanLy = 2;
            // Xử lý các đơn đặt phòng liên quan
            var datPhongs = db.DatPhongs.Where(dp => dp.MaNv == maNhanVien).ToList();
            foreach (var datPhong in datPhongs)
            {

                datPhong.MaNv = NhanVienQuanLy;
            }

            db.SaveChanges(); // Lưu các thay đổi

            // Xóa nhân viên
            db.NhanViens.Remove(nhanVien);
            db.SaveChanges();

            TempData["Message"] = "Đã sa thải nhân viên.";
            return RedirectToAction("Staff");
        }

    }
}
