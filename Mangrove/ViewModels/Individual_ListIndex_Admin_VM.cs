namespace Mangrove.ViewModels {
	public class Individual_ListIndex_Admin_VM {

		public string Id { get; set; } = null!;
		public string NameEN { get; set; } = null!;
		public string NameVI { get; set; } = null!;
		public string PositionEN { get; set; } = null!;
		public string PositionVI { get; set; } = null!;
		public int TotalStage { get; set; }
		public DateTime UpdateLast { get; set; }
	}
}
