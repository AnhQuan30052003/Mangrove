using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblPhoto
{
    public string Id { get; set; } = null!;

    public string IdObj { get; set; } = null!;

    public string ImageName { get; set; } = null!;

    public string? NoteImg { get; set; }
}
