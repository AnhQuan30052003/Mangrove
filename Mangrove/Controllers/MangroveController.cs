using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		public IActionResult Page_Index() {
			return View();
		}
	}
}
