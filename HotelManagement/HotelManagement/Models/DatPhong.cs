using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class DatPhong
{
    public int MaPhieuThue { get; set; }

    public int? MaKh { get; set; }

    public int? MaNv { get; set; }

    public int? MaLoaiHinhDat { get; set; }

    public DateOnly? NgayNhan { get; set; }

    public DateOnly? NgayTra { get; set; }

    public TimeOnly? GioNhan { get; set; }

    public TimeOnly? GioTra { get; set; }

    public int? MaPhong { get; set; }

    public int? SoNguoiO { get; set; }

    public int? MaTinhTrangDat { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual LoaiHinhDat? MaLoaiHinhDatNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual Phong? MaPhongNavigation { get; set; }

    public virtual TinhTrangDat? MaTinhTrangDatNavigation { get; set; }

    public virtual ICollection<PhongDichVu> PhongDichVus { get; set; } = new List<PhongDichVu>();

    public virtual PhongHuy? PhongHuy { get; set; }
}
