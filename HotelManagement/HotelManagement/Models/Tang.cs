using System;
using System.Collections.Generic;

namespace HotelManagement.Models;

public partial class Tang
{
    public int MaTang { get; set; }

    public string? TenTang { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
