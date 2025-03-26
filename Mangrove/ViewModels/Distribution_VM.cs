
namespace Mangrove.ViewModels {
	public class Distribution_VM {
		public string MapNameEn { get; set; } = null!;
		public string MapNameVi { get; set; } = null!;
		public DataImage Image { get; set; } = null!;
	}

	public class DataImage {
 		public string DataType { get; set; } = null!;
 		public string DataBase64 { get; set; } = null!;
	}
}
