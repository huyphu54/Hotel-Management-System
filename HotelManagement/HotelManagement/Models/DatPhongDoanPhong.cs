using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class DatPhongDoanPhong
{
    public int MaChiTiet { get; set; }

    public int? MaDoan { get; set; }

    public int? MaPhong { get; set; }

    public virtual DatPhongDoan? MaDoanNavigation { get; set; }

    public virtual Phong? MaPhongNavigation { get; set; }
}
