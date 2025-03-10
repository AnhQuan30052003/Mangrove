using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class AdminController : Controller {
		public IActionResult Page_Index() {
			return View();
		}
	}
}
