using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Mangrove.Data;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Controllers {
	public class SettingWebsiteController : Controller {
		private readonly MangroveContext context;
		
		public SettingWebsiteController(MangroveContext context) {
			this.context = context;
		}

		// Thay đổi ngôn ngữ Website
		public IActionResult ChangeLanguage(string? language) {
			if (!string.IsNullOrEmpty(language)) {
				// Tạo cookie lưu ngôn ngữ
				Response.Cookies.Append(
					CookieRequestCultureProvider.DefaultCookieName,
					CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
					new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
				);
			}
			return Redirect(Request.Headers["Referer"].ToString());
		}

		// Lấy dữ liệu cơ bản/mặc định cho website
		public async Task<TblSetting> GetSetting() {
			var setting = await context.TblSettings.FirstOrDefaultAsync();
			return setting ?? new TblSetting();
		}

		// Page không tồn tại
		public IActionResult Page_NotExists() {
			return View();
		}

		// Page không có quyền truy cập
		public IActionResult Page_NotAccess() {
			return View();
		}

		// Page không thể kết nối với Database
		public IActionResult Page_DisconnectDatabase() {
			return View();
		}
	}
}
