using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class PhongDichVu
{
    public int MaChiTiet { get; set; }

    public int MaPhieuThue { get; set; }

    public int MaDichVu { get; set; }

    public int? SoLuong { get; set; }

    public virtual DichVu MaDichVuNavigation { get; set; } = null!;

    public virtual DatPhong MaPhieuThueNavigation { get; set; } = null!;
}
