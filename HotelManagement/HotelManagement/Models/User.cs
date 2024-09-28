using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class User
{
    public int Uid { get; set; }

    public string? TaiKhoan { get; set; }

    public string? MatKhau { get; set; }

    public int MaNv { get; set; }

    public string? Role { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
