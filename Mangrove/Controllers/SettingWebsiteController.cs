using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Mangrove.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
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
		public async Task<IActionResult> Page_Error(string? typeError = null) {
			bool isEN = Helper.Func.IsEnglish();
			bool justBackPageLogin = false;
			if (typeError == null) {
				typeError = Helper.Variable.TypeError.notPermission;
				justBackPageLogin = true;
			}

			// Setup for not permisstion
			string EN = "You do not have permission to access this site !";
			string VI = "Bạn không có quyền truy cập trang web này !";

			var setting = await GetSetting();
			string image = "notPermission.png";
			string text = isEN ? EN : VI;

			if (typeError == Helper.Variable.TypeError.notExists) {
				EN = "You have accessed a web page that does not exist on our website !";
				VI = "Bạn đã truy cập một trang web không tồn tại trong website của chúng tôi !";
				image = setting.LogoImg;
				text = isEN ? EN : VI;
			}
			else if (typeError == Helper.Variable.TypeError.disconnectDatabase) {
				EN = "Error connecting to Database !";
				VI = "Có lỗi khi kết nối đến Cơ sở dữ liệu !";
				image = "disconnnetDatabase.png";
				text = isEN ? EN : VI;
			}

			ViewData["Image"] = image;
			ViewData["Text"] = text;
			ViewData["JustBackPageLogin"] = justBackPageLogin;
			return View();
		}

		// Page thông tin website (view)
		public async Task<IActionResult> WebsitInformation() {
			var setting = await GetSetting();
			return View(setting);
		}
	}
}
