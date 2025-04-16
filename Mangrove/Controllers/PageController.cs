using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class PageController : Controller {
	private readonly MangroveContext context;

		public PageController(MangroveContext context) {
			this.context = context;
		}

		public IActionResult Index() {
			return View();
		}
	}
}
