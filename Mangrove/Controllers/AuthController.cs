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
            ViewBag.account = account;
            TempData["Status"] = Helper.StatusNoifier.success;
            TempData["Content"] = "Chúng tôi đã gửi một mã 6 số tới email anhquan300503@gmail.com";
            TempData["Timer"] = 3000;

            return View();
        }

        public IActionResult Page_ForgottenPassword_Find() {
            return View();
        }

        public IActionResult Page_ForgottenPassword_Input() {
            return View();
        }

        public IActionResult Page_ChangePassword() {
            return View();
        }
    }
}