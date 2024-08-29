using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class LoaiPhong
{
    public int MaLp { get; set; }

    public string? TenLoaiPhong { get; set; }

    public string? TienNghi { get; set; }

    public double? SoTienCoc { get; set; }

    public double? Gia { get; set; }
    public int SoNguoiOToiDa { get; set; }
    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
