using Mangrove.Data;

namespace Mangrove.ViewModels {
	public class InfoStagesOfIndividual_Client_VM {
		public string IdIndividual { get; set; } = null!;
		public string NameMangrove { get; set; } = null!;
		public TblIndividual Individual { get; set; } = null!;

		public List<Stage> Stages { get; set; } = null!;
	}

	public class Stage {
		public TblStage info { get; set; } = null!;
		public List<TblPhoto> photo { get; set; } = null!;

	}
}
