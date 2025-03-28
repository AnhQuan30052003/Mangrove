using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class AdminController : Controller {
		public IActionResult Page_Statistical() {
			return View();
		}

		public IActionResult Page_AdminInformation() {
			return View();
		}

		public IActionResult Page_ChangePassword() {
			return View();
		}
	}
}
