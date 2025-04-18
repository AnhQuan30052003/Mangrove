using System;
using System.Collections.Generic;

namespace Mangrove.Data;

public partial class TblHome
{
    public string Id { get; set; } = null!;

    public string BannerImg { get; set; } = null!;

    public string TitleListItemVi { get; set; } = null!;

    public string TitleListItemEn { get; set; } = null!;

    public int ItemRecent { get; set; }

    public string BannerTitleVi { get; set; } = null!;

    public string BannerTitleEn { get; set; } = null!;

    public string TitlePurposeVi { get; set; } = null!;

    public string TitlePurposeEn { get; set; } = null!;

    public string PurposeVi { get; set; } = null!;

    public string PurposeEn { get; set; } = null!;
}
