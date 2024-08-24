using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class LoaiHinhDat
{
    public int MaLoaiHinhDat { get; set; }

    public string? TenLoaiHinhDat { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
