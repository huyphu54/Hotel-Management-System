using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class PhongBan
{
    public int MaPb { get; set; }

    public string? TenPb { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
