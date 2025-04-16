using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblDistributiton
{
    public string Id { get; set; } = null!;

    public string ImageMap { get; set; } = null!;

    public string MapNameVi { get; set; } = null!;

    public string MapNameEn { get; set; } = null!;

    public DateTime? UpdateLast { get; set; }
}
