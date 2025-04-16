using Mangrove.Data;

namespace Mangrove.ViewModels {
	public class Statistical_Filter {
		public string ControllerName { get; set; } = null!;
		public string ActionName { get; set; } = null!;
		public Paginate_VM<TblMangrove> Mangrove { get; set; } = null!;
		public Paginate_VM<TblIndividual> Individual { get; set; } = null!;
	}
}
