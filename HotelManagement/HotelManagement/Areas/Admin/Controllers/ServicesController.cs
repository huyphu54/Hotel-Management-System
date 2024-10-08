using HotelManagement.Filters;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
namespace HotelManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        QlksContext db = new QlksContext();
        [Route("Admin/Services")]
        [HttpGet]
        public IActionResult Services(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var listDichVu = db.DichVus.Include(ldv=>ldv.MaLoaiDvNavigation).AsNoTracking().OrderBy(x => x.MaDichVu);
            PagedList<DichVu> lstDicVu = new PagedList<DichVu>(listDichVu, pageNumber, pageSize); 
            return View(lstDicVu);
        }
        [Route("Admin/AddServices")]
        [HttpGet]
        public IActionResult AddServices()
        {
            ViewBag.MaLoaiDv = new SelectList(db.LoaiDichVus.ToList(), "MaLoaiDv", "TenLoaiDv");
            return View();
        }
        [Route("Admin/AddServices")]
        [HttpPost]
        public IActionResult AddServices(DichVu dv)
        {

            if (ModelState.IsValid)
            {
                db.DichVus.Add(dv);
                db.SaveChanges();               
                return RedirectToAction("Services");
            }
            ViewBag.MaLoaiDv = new SelectList(db.LoaiDichVus.ToList(), "MaLoaiDv", "TenLoaiDv");
            return View(dv);
        }
        [Route("Admin/UpdateServices")]
        [HttpGet]
        public IActionResult UpdateServices(int? maDV)
        {
           
            ViewBag.MaLoaiDv = new SelectList(db.LoaiDichVus.ToList(), "MaLoaiDv", "TenLoaiDv");
            var dichVu = db.DichVus.Find(maDV);
            return View(dichVu);
        }
        [Route("Admin/UpdateServices")]
        [HttpPost]
        public IActionResult UpdateServices(DichVu dv)
        {
            var updateDichVu = db.DichVus.Find(dv.MaDichVu);
            if (updateDichVu == null)
            {
                return NotFound();
            }
            updateDichVu.Gia = dv.Gia;
            updateDichVu.TinhTrang = dv.TinhTrang;
            if (ModelState.IsValid)
            {
                db.Entry(updateDichVu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật dịch vụ thành công";
                return RedirectToAction("Services");
            }
            TempData["Message"] = "Cập nhật dịch vụ không thành công";
            ViewBag.MaLoaiDv = new SelectList(db.LoaiDichVus.ToList(), "MaLoaiDv", "TenLoaiDv");
            return View(dv);
        }
       

    }
}
