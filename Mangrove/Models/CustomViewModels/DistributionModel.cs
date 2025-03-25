//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using System.ComponentModel.DataAnnotations;

namespace Mangrove.Models {
	public class DistributionModel {
		//[BindNever]
		public string ImageMap { get; set; } = null!;

		public string MapNameVi { get; set; } = null!;

		public string MapNameEn { get; set; } = null!;
	}
}
