using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mangrove.Controllers {
	public class AuthController : Controller {
		private readonly MangroveContext context;

		public AuthController(MangroveContext context) {
			this.context = context;
		}

		// Đăng nhập
		public IActionResult Page_Login() {
			return View();
		}
		[HttpPost]
		public IActionResult Page_Login(string account, string password) {
			ViewData["Account"] = account;
			ViewData["Password"] = password;

			bool isEN = Helper.Func.IsEnglish();
			try {
				// Beign validate
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(account);
				Helper.Validate.NotEmpty(password);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				Helper.Notifier.Success(
					isEN ? "Login successfully." : "Đăng nhập thành công.",
					Helper.SetupNotifier.Timer.fastTime,
					Url.Action("Page_Statistical", "Admin")
				);

				return View();
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Login request failed. Please try again later !" : "Gửi yêu cầu đăng nhập thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View();
			}
		}

		// Quên mật khẩu - Tìm
		public IActionResult Page_ForgottenPassword_Find() {
			return View();
		}

		// Quên mật khẩu - Nhập dữ liệu
		public IActionResult Page_ForgottenPassword_Input() {
			return View();
		}

		// Đăng xuất
		public IActionResult Page_Logout() {
			return RedirectToAction("Page_Index", "Home");
		}
	}
}