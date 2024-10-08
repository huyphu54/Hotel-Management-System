using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class LoaiDichVu
{
    public int MaLoaiDv { get; set; }

    public string? TenLoaiDv { get; set; }

    public virtual ICollection<DichVu> DichVus { get; set; } = new List<DichVu>();
}
