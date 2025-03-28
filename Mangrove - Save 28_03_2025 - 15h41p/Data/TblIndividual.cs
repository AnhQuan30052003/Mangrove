using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblIndividual
{
    public string Id { get; set; } = null!;

    public string? IdMangrove { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public string PositionVi { get; set; } = null!;

    public string PositionEn { get; set; } = null!;

    public DateTime UpdateLast { get; set; }

    public string QrName { get; set; } = null!;

    public long View { get; set; }

    public virtual TblMangrove? IdMangroveNavigation { get; set; }

    public virtual ICollection<TblStage> TblStages { get; set; } = new List<TblStage>();
}
