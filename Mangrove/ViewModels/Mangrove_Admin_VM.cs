using Mangrove.Validates;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Mangrove.ViewModels {
	public class Mangrove_Admin_VM {
		public string NameEn { get; set; } = null!;
		public string NameVi { get; set; } = null!;

		public string CommonNameEn { get; set; } = null!;
		public string CommonNameVi { get; set; } = null!;

		public string ScientificName { get; set; } = null!;
		public string Familia { get; set; } = null!;

		public string MorphologyEn { get; set; } = null!;
		public string MorphologyVi { get; set; } = null!;

		public string EcologyEn { get; set; } = null!;
		public string EcologyVi { get; set; } = null!;
		
		public string DistributionEn { get; set; } = null!;
		public string DistributionVi { get; set; } = null!;

		public string ConservationStatusEn { get; set; } = null!;
		public string ConservationStatusVi { get; set; } = null!;

		public string UseEn { get; set; } = null!;
		public string UseVi { get; set; } = null!;


		public List<Photo_Mangrove_VM> Photos { get; set; } = new List<Photo_Mangrove_VM>();
	}

	public class Photo_Mangrove_VM {
		public DataImage Image { get; set; } = null!;
		public string NoteImgEn { get; set; } = null!;
		public string NoteImgVi { get; set; } = null!;
	}
}
