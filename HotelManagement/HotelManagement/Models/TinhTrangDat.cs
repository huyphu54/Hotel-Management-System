using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class TinhTrangDat
{
    public int MaTinhTrangDat { get; set; }

    public string? TenTinhTrangDat { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
