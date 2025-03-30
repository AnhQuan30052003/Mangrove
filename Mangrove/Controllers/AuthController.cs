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

		public IActionResult Page_Login() {
			return View();
		}
		[HttpPost]
		public IActionResult Page_Login(string account, string password) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				TempData["Account"] = account;

				Helper.Notifier.Success(
					isEN ? "Login successfully." : "Đăng nhập thành công.",
					Helper.SetupNotifier.Timer.fastTime,
					Url.Action("Page_Statistical", "Admin")
				);
				return RedirectToAction("Page_Login");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Login request failed. Please try again later !" : "Gửi yêu cầu đăng nhập thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Login");
			}
		}

		public IActionResult Page_ForgottenPassword_Find() {
			return View();
		}

		public IActionResult Page_ForgottenPassword_Input() {
			return View();
		}
	}
}