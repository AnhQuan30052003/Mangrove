using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblStage
{
    public string Id { get; set; } = null!;

    public string? IdIndividual { get; set; }

    public string MainImage { get; set; } = null!;

    public DateTime SurveyDay { get; set; }

    public string NameVi { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string? WeatherVi { get; set; }

    public string? WeatherEn { get; set; }

    public int? NumberOrder { get; set; }

    public string? Height { get; set; }

    public string? Perimeter { get; set; }

    public virtual TblIndividual? IdIndividualNavigation { get; set; }
}
