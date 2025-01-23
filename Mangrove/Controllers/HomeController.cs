using System.Diagnostics;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger) {
            this.logger = logger;
        }

        public IActionResult Index() {
            return View();
        }
    }
}
