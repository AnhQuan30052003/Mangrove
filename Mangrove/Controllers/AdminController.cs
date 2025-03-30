using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Mangrove.Controllers {
	public class AdminController : Controller {

		private readonly MangroveContext context;

		public AdminController(MangroveContext context) {
			this.context = context;
		}

		// Thống kê
		public IActionResult Page_Statistical() {
			return View();
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
				return Content(Helper.Link.GetUrlBack(Helper.Key.adminInfo), "text/html");
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
				return Content(Helper.Link.GetUrlBack(Helper.Key.adminInfo), "text/html");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_AdminInformation_Edit(TblAdmin model) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validae
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(model.Username);
				Helper.Validate.NotEmpty(model.Email);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Required fields are not filled in!" : "Các ô bắt buộc chưa được nhập !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				//await context.SaveChangesAsync();
				Helper.Notifier.Success(
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.fastTime
				);
				return Content(Helper.Link.GetUrlBack(Helper.Key.adminInfo), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
				isEN ? "Request to edit admin infomation status failed. Please try again later !" : "Gửi yêu cầu sửa thông tin quản trị thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.GetUrlBack(Helper.Key.adminInfo), "text/html");
			}
		}

		// Thay đổi mật khẩu
		public IActionResult Page_ChangePassword() {
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_ChangePassword(string oldPass, string newPass, string newPassConfirm) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Sticky data
				ViewData["OldPass"] = oldPass;
				ViewData["NewPass"] = newPass;
				ViewData["NewPassConfirm"] = newPassConfirm;

				// Beigin validate
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(oldPass);
				Helper.Validate.NotEmpty(newPass);
				Helper.Validate.NotEmpty(newPassConfirm);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Required fields are not filled in!" : "Các ô bắt buộc chưa được nhập !",
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
				//admin.Password = newPass;
				//await context.SaveChangesAsync();

				Helper.Notifier.Success(
					isEN ? "Change password successfully." : "Đổi mật khẩu thành công.",
					Helper.SetupNotifier.Timer.fastTime
				);
				return Content(Helper.Link.GetUrlBack(Helper.Key.changePassword), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Change password request failed. Please try again later !" : "Yêu cầu đổi mật khẩu thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(Helper.Key.changePassword), "text/html");
			}
		}
	}
}
