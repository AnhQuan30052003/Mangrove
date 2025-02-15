using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class HomeController : Controller {
		public HomeController() {

		}
		public IActionResult Index() {
			//TempData["Status"] = Helper.StatusNoifier.success;
			//TempData["Content"] = "Thành công test code";

			return View();
		}

		public IActionResult Results() {
			return View();
		}

		public IActionResult Result(string id) {
			return View();
		}
	}
}
