using Mangrove.Data;

namespace Mangrove.ViewModels {
	public class HomePage_Client_VM {
		public string BannerTitle { get; set; } = null!;
		public string BannerImage { get; set; } = null!;
		public string PurposeTitle { get; set; } = null!;
		public string PurposeContent { get; set; } = null!;
		public string LabelSpeciesCompositionRecent { get; set; } = null!;

		public List<Mangrove_HomePage_Client_VM> Mangroves = new List<Mangrove_HomePage_Client_VM>();
		public List<TblPhoto> Sponsor = new List<TblPhoto>();
	}

	public class Mangrove_HomePage_Client_VM {
		public string Id { get; set; } = null!;
		public string Image { get; set; } = null!;
		public string Name { get; set; } = null!;
	}
}
