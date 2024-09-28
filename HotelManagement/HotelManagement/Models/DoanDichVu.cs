using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class DoanDichVu
{
    public int MaChiTietDv { get; set; }

    public int? MaDoan { get; set; }

    public int? MaDichVu { get; set; }

    public int? SoLuong { get; set; }

    public virtual DichVu? MaDichVuNavigation { get; set; }

    public virtual DatPhongDoan? MaDoanNavigation { get; set; }
}
