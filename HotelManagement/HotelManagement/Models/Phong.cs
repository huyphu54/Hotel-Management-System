using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class Phong
{
    public int MaPhong { get; set; }

    public string? SoPhong { get; set; }

    public int MaTang { get; set; }

    public int MaLp { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual LoaiPhong MaLpNavigation { get; set; } = null!;

    public virtual Tang MaTangNavigation { get; set; } = null!;
}
