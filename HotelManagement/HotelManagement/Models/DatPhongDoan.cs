using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class DatPhongDoan
{
    public int MaDoan { get; set; }

    public string? TenDoan { get; set; }

    public DateOnly? NgayDat { get; set; }

    public DateOnly? NgayTra { get; set; }

    public int? SoNguoiO { get; set; }

    public int? MaKh { get; set; }

    public int? MaTinhTrangDat { get; set; }

    public int? MaNv { get; set; }

    public virtual ICollection<DatPhongDoanPhong> DatPhongDoanPhongs { get; set; } = new List<DatPhongDoanPhong>();

    public virtual ICollection<DoanDichVu> DoanDichVus { get; set; } = new List<DoanDichVu>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual TinhTrangDat? MaTinhTrangDatNavigation { get; set; }
}
