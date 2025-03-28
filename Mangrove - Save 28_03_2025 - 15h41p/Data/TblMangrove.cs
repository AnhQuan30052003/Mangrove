using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblMangrove
{
    public string Id { get; set; } = null!;

    public string NameVi { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public string CommonNameVi { get; set; } = null!;

    public string CommonNameEn { get; set; } = null!;

    public string ScientificName { get; set; } = null!;

    public string Familia { get; set; } = null!;

    public string MainImage { get; set; } = null!;

    public string MorphologyVi { get; set; } = null!;

    public string MorphologyEn { get; set; } = null!;

    public string EcologyVi { get; set; } = null!;

    public string EcologyEn { get; set; } = null!;

    public string DistributionVi { get; set; } = null!;

    public string DistributionEn { get; set; } = null!;

    public string ConservationStatusVi { get; set; } = null!;

    public string ConservationStatusEn { get; set; } = null!;

    public string UseVi { get; set; } = null!;

    public string UseEn { get; set; } = null!;

    public long View { get; set; }

    public DateTime UpdateLast { get; set; }

    public virtual ICollection<TblIndividual> TblIndividuals { get; set; } = new List<TblIndividual>();
}
