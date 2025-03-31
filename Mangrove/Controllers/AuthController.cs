using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Collections.Generic;

namespace Mangrove.Controllers {
	public class AuthController : Controller {
		private readonly MangroveContext context;

		public AuthController(MangroveContext context) {
			this.context = context;
		}

		// Đăng nhập
		public IActionResult Page_Login() {
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_Login(string username, string password) {
			ViewData["Username"] = username;
			ViewData["Password"] = password;

			bool isEN = Helper.Func.IsEnglish();
			try {
				// Beign validate
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(username);
				Helper.Validate.NotEmpty(password);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : "Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Check đăng nhập
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				if ((admin!.Username == username || admin!.Email == username) && (admin!.Password == password)) {
					var principal = new ClaimsPrincipal(
					new ClaimsIdentity(
						new List<Claim>
							{
								new Claim("Username", username),
								new Claim("Password", password)
							},
							Helper.Variable.cookieName
						)
					);
					await HttpContext.SignInAsync(Helper.Variable.cookieName, principal);

					ViewData["Username"] = ViewData["Password"] = string.Empty;

					Helper.Notifier.Success(
						isEN ? "Login successfully. Redirecting..." : "Đăng nhập thành công. Đang chuyển hướng...",
						Helper.SetupNotifier.Timer.fastTime,
						Url.Action("Page_Statistical", "Admin")
					);
					return View();
				}

				Helper.Notifier.Fail(
					isEN ? "Username or passoword incorect !" : "Tài khoản hoặc mật khẩu không chính xác !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return View();
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Login request failed. Please try again later !" : "Gửi yêu cầu đăng nhập thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View();
			}
		}

		// Quên mật khẩu - Tìm
		public IActionResult Page_ForgottenPassword_Find() {
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public IActionResult Page_ForgottenPassword_Find(string email) {
			ViewData["Email"] = email;
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validate
				Helper.Validate.IsEmail(email);

				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Tạo mã 6 số
				// Gửi email thông báo

				// Setup thông báo thàh công
				Helper.Notifier.Success(
					isEN ? "Please check your email for a password reset code." : "Hãy kiểm tra email để lấy mã tạo lại mật khẩu.",
					Helper.SetupNotifier.Timer.longTime
				);
				return RedirectToAction("Page_ForgottenPassword_Input", new { access = true });
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Password recovery request failed. Please try again later !" : "Gửi yêu cầu khôi phục mật khẩu thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View();
			}
		}

		// Quên mật khẩu - Nhập dữ liệu
		public IActionResult Page_ForgottenPassword_Input(bool access = false) {
			if (!access) {
				return RedirectToAction("Page_ForgottenPassword_Find");
			}

			return View();
		}
		[HttpPost]
		public IActionResult Page_ForgottenPassword_Input(string codeNumber, string newPass, string newPassConfirm) {
			bool isEN = Helper.Func.IsEnglish();
			// Check mã 6 số

			// Check mật khẩu xác nhận

			
			return RedirectToAction("Page_Login");
		}


		// Đăng xuất
		[Authorize]
		public async Task<IActionResult> Page_Logout() {
			await HttpContext.SignOutAsync(Helper.Variable.cookieName);
			return RedirectToAction("Page_Index", "Home");
		}
	}
}