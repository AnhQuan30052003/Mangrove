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

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string account, string password) {
            ViewBag.account = account;
            TempData["Status"] = Helper.StatusNoifier.success;
            TempData["Content"] = "Đăng nhập thành công";

            return View();
        }

        public IActionResult ForgottenPassword_Find() {
            return View();
        }

        public IActionResult ForgottenPassword_Input() {
            return View();
        }

        public IActionResult ChangePassword() {
            return View();
        }
    }
}