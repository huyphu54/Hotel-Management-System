using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public int MaPhieuThue { get; set; }

    public int? SoNgayO { get; set; }

    public double? SoTienCoc { get; set; }

    public double? PhuThu { get; set; }

    public double? TongTienTamTinh { get; set; }

    public double? TongTienThu { get; set; }

    public string? TinhTrang { get; set; }

    public virtual DatPhong MaPhieuThueNavigation { get; set; } = null!;
}
