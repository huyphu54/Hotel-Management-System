using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class HoaDon
{
    public int MaHoaDon { get; set; }

    public int? MaPhieuThue { get; set; }

    public int? MaDoan { get; set; }

    public DateOnly? NgayLapPhieu { get; set; }

    public double? SoTienCoc { get; set; }

    public double? TienDichVu { get; set; }

    public double? PhuThu { get; set; }

    public double? TongTienTamTinh { get; set; }

    public double? TongTienThu { get; set; }

    public virtual DatPhongDoan? MaDoanNavigation { get; set; }

    public virtual DatPhong? MaPhieuThueNavigation { get; set; }
}
