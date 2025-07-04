﻿using DocumentFormat.OpenXml.Spreadsheet;
using Mangrove.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Controllers {
	[Authorize]
	public class AdminController : Controller {

		private readonly MangroveContext context;

		public AdminController(MangroveContext context) {
			this.context = context;
		}

		// Xem thông tin 
		public async Task<IActionResult> Page_AdminInformation_View() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				Helper.Validate.Clear();
				return View(admin);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The admin information page is currently inaccessible !" : "Trang thông tin quản trị hiện tại không thể truy cập !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageInfo), "text/html");
			}
		}

		// Sửa thông tin 
		public async Task<IActionResult> Page_AdminInformation_Edit() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				Helper.Validate.Clear();
				return View(admin);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The edit admin information page is currently inaccessible !" : "Trang chỉnh sửa thông tin quản trị hiện tại không thể truy cập !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return RedirectToAction("Page_AdminInformation_View");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_AdminInformation_Edit(TblAdmin model) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				if (!string.IsNullOrEmpty(model.Username)) {
					model.Username = model.Username.Trim();
				}
				if (!string.IsNullOrEmpty(model.Email)) {
					model.Email = model.Email.Trim();
				}
				if (!string.IsNullOrEmpty(model.CodeSendEmail)) {
					model.CodeSendEmail = model.CodeSendEmail.Trim();
				}

				// Begin validae
				Helper.Validate.Clear();
				Helper.Validate.TextLength(model.Username, 4, 30);
				Helper.Validate.IsEmail(model.Email);
				Helper.Validate.TextLength(model.CodeSendEmail, 19);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Lưu dữ liệu
				context.TblAdmins.Update(model);
				await context.SaveChangesAsync();

				string subject = isEN ? "UPDATE ADMIN INFORMATION" : "CẬP NHẬT THÔNG TIN QUẢN TRỊ";
				string body = Helper.Email.CreateFormHtmlNotifierStatusAdminInformatoin(model.Username, model.Email, model.CodeSendEmail);
				Helper.Email.SendAsync(model.Email, model.CodeSendEmail, model.Email, subject, body);
				Helper.Notifier.Success(
					isEN ? "The system will send an email notifying you of a successful update. If you do not receive the notification, please check your information again." 
					: "Hệ thống sẽ gửi một email thông báo cập nhật thành công. Nếu không nhận được thông báo, vui lòng kiểm tra lại thông tin.",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_AdminInformation_View");
			}
			catch {
				Helper.Notifier.Fail(
				isEN ? "Request to edit admin infomation status failed. Please try again later !" : "Gửi yêu cầu sửa thông tin quản trị thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return RedirectToAction("Page_AdminInformation_View");
			}
		}

		// Thay đổi mật khẩu
		public IActionResult Page_ChangePassword() {
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_ChangePassword(string oldPass = "", string newPass = "", string newPassConfirm = "") {
			bool isEN = Helper.Func.IsEnglish();
			try {
				oldPass = oldPass.Trim();
				newPass = newPass.Trim();
				newPassConfirm = newPassConfirm.Trim();

				// Sticky data
				ViewData["OldPass"] = oldPass;
				ViewData["NewPass"] = newPass;
				ViewData["NewPassConfirm"] = newPassConfirm;

				// Beigin validate
				Helper.Validate.Clear();
				Helper.Validate.TextLength(oldPass, 4, 40);
				Helper.Validate.TextLength(newPass, 4, 40);
				Helper.Validate.TextLength(newPassConfirm, 4, 40);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Check dữ liệu
				var admin = await context.TblAdmins.FirstOrDefaultAsync();
				if (admin!.Password != oldPass) {
					Helper.Notifier.Fail(
						isEN ? "Old password is incorret !" : "Mật khẩu cũ không chính xác !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				if (newPass != newPassConfirm) {
					Helper.Notifier.Fail(
						isEN ? "Mật khẩu xác nhận không khớp !" : "Mật khẩu xác nhận không khớp !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Lưu dữ liệu
				admin.Password = newPass;
				context.TblAdmins.Update(admin);
				await context.SaveChangesAsync();

				Helper.Notifier.Success(
					isEN ? "Change password successfully." : "Đổi mật khẩu thành công.",
					Helper.SetupNotifier.Timer.fastTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageChangePassword), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Change password request failed. Please try again later !" : "Yêu cầu đổi mật khẩu thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageChangePassword), "text/html");
			}
		}
	}
}
