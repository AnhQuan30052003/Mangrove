using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblStage
{
    public string Id { get; set; } = null!;

    public string? IdIndividual { get; set; }

    public virtual TblIndividual? IdIndividualNavigation { get; set; }
}
