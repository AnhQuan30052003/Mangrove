using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class LanguageController : Controller {
		public IActionResult Change(string? language) {
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
	}
}
