using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class PhongHuy
{
    public string MaPhongHuy { get; set; } = null!;

    public int MaPhieuThue { get; set; }

    public string? LyDo { get; set; }

    public DateOnly? NgayHuy { get; set; }

    public TimeOnly? GioHuy { get; set; }

    public virtual DatPhong MaPhieuThueNavigation { get; set; } = null!;
}
