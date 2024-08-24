using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class NhanVien
{
    public int MaNv { get; set; }

    public string HoTenNv { get; set; } = null!;

    public string? Email { get; set; }

    public string? Sdt { get; set; }

    public string MaPb { get; set; } = null!;

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual PhongBan MaPbNavigation { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
