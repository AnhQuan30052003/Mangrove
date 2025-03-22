using Mangrove.Validates;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Mangrove.Models {
	public class MangroveModel {
		[ValidateCustom(ValidateCustom.Type.NotEmpty)]
		public string NameVi { get; set; } = null!;


		[ValidateCustom(ValidateCustom.Type.NotEmpty)]
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

		public List<PhotoModel> Photos { get; set; } = new List<PhotoModel>();
	}

	public class PhotoModel {
		public string Image { get; set; } = null!;

		[ValidateCustom(ValidateCustom.Type.NotEmpty)]

		public string NoteImgEn { get; set; } = null!;

		[ValidateCustom(ValidateCustom.Type.NotEmpty)]

		public string NoteImgVi { get; set; } = null!;
	}
}
