using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class PhongHuy
{
    public int MaPhongHuy { get; set; }

    public int MaPhieuThue { get; set; }

    public string? LyDo { get; set; }

    public DateOnly? NgayHuy { get; set; }

    public virtual DatPhong MaPhieuThueNavigation { get; set; } = null!;
}
