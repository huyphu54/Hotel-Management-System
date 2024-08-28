using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class TinhTrangPhong
{
    public int MaTinhTrang { get; set; }

    public string? TenTinhTrang { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
