using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblMangrove
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string OtherName { get; set; } = null!;

    public string ScientificName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Morphology { get; set; } = null!;

    public string Ecology { get; set; } = null!;

    public string Distribution { get; set; } = null!;

    public string ConservationStatus { get; set; } = null!;

    public string Use { get; set; } = null!;

    public int Quantity { get; set; }

    public int View { get; set; }

    public virtual ICollection<TblIndividual> TblIndividuals { get; set; } = new List<TblIndividual>();
}
