using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
    public class HomeController : Controller {
        public HomeController() {

        }
        public IActionResult Index() {
            return View();
        }
    }
}
