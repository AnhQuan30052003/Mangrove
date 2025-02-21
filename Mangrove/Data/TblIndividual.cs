using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblIndividual
{
    public string Id { get; set; } = null!;

    public string? IdMangrove { get; set; }

    public DateTime SurveyDay { get; set; }

    public string Position { get; set; } = null!;

    public string QrName { get; set; } = null!;

    public long View { get; set; }

    public virtual TblMangrove? IdMangroveNavigation { get; set; }

    public virtual ICollection<TblStage> TblStages { get; set; } = new List<TblStage>();
}
