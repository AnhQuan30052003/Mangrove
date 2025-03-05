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
		public string GetSetting() {
			//var setting = await context.TblIndividuals.FirstOrDefaultAsync();
			//return setting ?? new TblIndividual();
			return "GetSetting Success.";
		}
	}
}
