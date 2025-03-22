using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mangrove.Validates {
	public class ValidateCustom : ValidationAttribute {
		// Kiểm tra tên và họ không chứa ký tự đặc biệt
		private readonly bool isEN = Helper.Func.IsLanguage("en");

		private Type type;

		public enum Type {
			NotEmpty
		}

		public ValidateCustom(Type type) {
			this.type = type;
		}

		protected override ValidationResult? IsValid(object value, ValidationContext context) {

			if (type == Type.NotEmpty) {
				if (string.IsNullOrEmpty(value as string)) {
					string EN = "Can't be blank !";
					string VI = "Không được bỏ trống !";
					return new ValidationResult(isEN ? EN : VI);
				}
			}

			return ValidationResult.Success;
		}
	}
}
