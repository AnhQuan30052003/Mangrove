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
			TempData["Account"] = account;

			string textNotifier = Helper.Func.IsLanguage("EN") ? "Login successfully." : "Đăng nhập thành công.";
			Helper.Notifier.Create(
				Helper.SetupNotifier.Status.success,
				textNotifier,
				Helper.SetupNotifier.Timer.fastTime,
				"/Admin/Page_Statistical"
			);

			return RedirectToAction(nameof(Page_Login));
		}

		public IActionResult Page_ForgottenPassword_Find() {
			return View();
		}

		public IActionResult Page_ForgottenPassword_Input() {
			return View();
		}
	}
}