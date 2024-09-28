using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class DichVu
{
    public int MaDichVu { get; set; }

    public string? TenDichVu { get; set; }

    public double? Gia { get; set; }

    public virtual ICollection<DoanDichVu> DoanDichVus { get; set; } = new List<DoanDichVu>();

    public virtual ICollection<PhongDichVu> PhongDichVus { get; set; } = new List<PhongDichVu>();
}
