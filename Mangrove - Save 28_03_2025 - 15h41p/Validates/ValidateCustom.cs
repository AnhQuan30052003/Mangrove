using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mangrove.Validates {
	//public class ValidateCustom : ValidationAttribute {
	//	private readonly bool isEN = Helper.Func.IsLanguage("en");

	//	private Type type;

	//	public enum Type {
	//		NotEmpty
	//	}

	//	public ValidateCustom(Type type) {
	//		this.type = type;
	//	}

	//	protected override ValidationResult? IsValid(object value, ValidationContext context) {
	//		Console.WriteLine("Đã laod 1");

	//		if (type == Type.NotEmpty) {
	//			Console.WriteLine("Đã laod 2");
	//			if (string.IsNullOrEmpty(value as string)) {
	//				string EN = "Can't be blank !";
	//				string VI = "Không được bỏ trống !";
	//				Console.WriteLine("Đã laod 3");
	//				return new ValidationResult(isEN ? EN : VI);
	//			}
	//		}

	//		return ValidationResult.Success;
	//	}
	//}

	public class ValidateCustomAttribute : ValidationAttribute {
		public enum Type { NotEmpty }

		private Type ValidationType { get; }

		public ValidateCustomAttribute(Type type) {
			ValidationType = type;
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
			Console.WriteLine($"IsValid() được gọi cho {validationContext.DisplayName} với giá trị: {value}");

			if (ValidationType == Type.NotEmpty && string.IsNullOrWhiteSpace(value?.ToString())) {
				return new ValidationResult("Không được bỏ trống!");
			}
			return ValidationResult.Success;
		}
	}

}
