using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblStage
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? IdIndividual { get; set; }

    public DateTime SurveyDay { get; set; }

    public string MainImage { get; set; } = null!;

    public virtual TblIndividual? IdIndividualNavigation { get; set; }
}
