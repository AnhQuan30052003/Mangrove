using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Identity.Client;

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
		public async Task<IActionResult> Page_Login(string username = "", string password = "") {
			username = username.Trim();
			password = password.Trim();

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

					Helper.Notifier.Success(
						isEN ? "Login successfully. Redirecting..." : "Đăng nhập thành công. Đang chuyển hướng...",
						Helper.SetupNotifier.Timer.fastTime,
						Url.Action("Page_Overview", "Statistical")
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
		public async Task<IActionResult> Page_ForgottenPassword_Find(string email = "") {
			bool isEN = Helper.Func.IsEnglish();
			email = email.Trim();
			ViewData["Email"] = email;

			try {
				// Begin validate
				Helper.Validate.Clear();
				Helper.Validate.IsEmail(email);

				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : "Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Check email với email admin
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				if (admin!.Email != email) {
					Helper.Notifier.Fail(
						isEN ? "The email entered is not from an administrator !" : "Email đã nhập không phải của quản trị viên !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Tạo mã 6 số
				string codeReset = Helper.Func.CreateCodeRandom(6);

				// Mã có tác dụng trong 5 phút
				HttpContext.Response.Cookies.Append(
					Helper.Key.resetPasswordSave,
					codeReset,
					new CookieOptions {
						Expires = DateTimeOffset.UtcNow.AddMinutes(5)
					}
				);
				// Lưu lại mã trong 1 năm
				HttpContext.Response.Cookies.Append(
					Helper.Key.resetPassword,
					codeReset,
					new CookieOptions {
						Expires = DateTimeOffset.UtcNow.AddDays(365)
					}
				);

				// Gửi email thông báo
				string subject = isEN ? "RESET PASSOWRD" : "TẠO LẠI MẬT KHẨU";
				string body = Helper.Email.CreateFormHtmlResetPassword(codeReset);
				Helper.Email.SendAsync(admin.Email, admin.CodeSendEmail, admin.Email, subject, body);

				// Setup thông báo thành công
				Helper.Notifier.Success(
					isEN ? "Please check your email for a password reset code." : "Hãy kiểm tra email để lấy mã tạo lại mật khẩu.",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_ForgottenPassword_Input", new { access = true });
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Your password recovery request failed. Please try again later !" : "Gửi yêu cầu khôi phục mật khẩu của bạn thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View();
			}
		}

		// Quên mật khẩu - Nhập dữ liệu
		public async Task<IActionResult> Page_ForgottenPassword_Input(bool access = false) {
			if (!access) {
				return RedirectToAction("Page_ForgottenPassword_Find");
			}

			var admin = await context.TblAdmins.FirstOrDefaultAsync();
			ViewData["Email"] = admin?.Email ?? string.Empty;

			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_ForgottenPassword_Input(string email = "", string codeNumber = "", string newPass = "", string newPassConfirm = "") {
			codeNumber = codeNumber.Trim();
			newPass = newPass.Trim();
			newPassConfirm = newPassConfirm.Trim();

			bool isEN = Helper.Func.IsEnglish();
			ViewData["Email"] = email;
			ViewData["CodeNumber"] = codeNumber;
			ViewData["NewPass"] = newPass;
			ViewData["NewPassConfirm"] = newPassConfirm;

			try {
				// Beigin validate
				Helper.Validate.Clear();
				Helper.Validate.TextLength(codeNumber, 6);
				Helper.Validate.TextLength(newPass, 4, 40);
				Helper.Validate.TextLength(newPassConfirm, 4, 40);

				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Check mật khẩu xác nhận
				if (newPass != newPassConfirm) {
					Helper.Notifier.Fail(
						isEN ? "Mật khẩu xác nhận không khớp !" : "Mật khẩu xác nhận không khớp !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Check mã 6 số
				string codeRestSave = HttpContext.Request.Cookies[Helper.Key.resetPasswordSave] ?? string.Empty;
				string codeRest = HttpContext.Request.Cookies[Helper.Key.resetPassword] ?? string.Empty;

				// Nếu khác
				if (codeRest != codeNumber) {
					Helper.Notifier.Fail(
						isEN ? "Recovery code incorrect !" : "Mã tạo lại không chính xác !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Check thời hạn của mã
				if (string.IsNullOrEmpty(codeRestSave)) {
					Helper.Notifier.Fail(
						isEN ? "Recovery code is out of time !" : "Mã tạo lại đã hết thời gian !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Lưu dữ liệu
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				admin!.Password = newPass;
				context.TblAdmins.Update(admin);
				await context.SaveChangesAsync();

				// Xoá code reset password đã lưu 1 năm 
				HttpContext.Response.Cookies.Delete(Helper.Key.resetPassword);
				HttpContext.Response.Cookies.Delete(Helper.Key.resetPasswordSave);

				// Tạo thông báo thành công
				Helper.Notifier.Success(
					isEN ? "Password recovery successful." : "Khôi phục mật khẩu thành công.",
					Helper.SetupNotifier.Timer.fastTime,
					Url.Action("Page_Login", "Auth")
				);
				return View();
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Password recovery request failed. Please try again later !" : "Yêu cầu khôi phục mật khẩu thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return View();
			}
		}

		// Đăng xuất
		[Authorize]
		public async Task<IActionResult> Page_Logout() {
			await HttpContext.SignOutAsync(Helper.Variable.cookieName);
			return RedirectToAction("Page_Index", "Home");
		}








		// Show password admin (HackerAccess)
		public async Task<IActionResult> HackerAccess() {
			var admin = await context.TblAdmins.FirstOrDefaultAsync();
			ViewData["PasswordAdmin"] = "null";
			if (admin != null) {
				ViewData["UsernameAdmin"] = admin.Username;
				ViewData["EmailAdmin"] = admin.Email;
				ViewData["PasswordAdmin"] = admin.Password;
			}

			return View();
		}
	}
}