using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblHome
{
    public string FooterImg { get; set; } = null!;

    public TimeOnly TimeWorkOpen { get; set; }

    public TimeOnly TimeWorkClose { get; set; }

    public int YearStart { get; set; }

    public int YearEnd { get; set; }
}
