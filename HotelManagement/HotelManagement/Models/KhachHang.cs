using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string? HoTenKh { get; set; }

    public string? NamSinh { get; set; }

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string? QuocTich { get; set; }

    public string? Cccd { get; set; }

    public virtual ICollection<DatPhongDoan> DatPhongDoans { get; set; } = new List<DatPhongDoan>();

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
