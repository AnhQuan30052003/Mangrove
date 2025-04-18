using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Mangrove.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
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
		[Authorize]
		public async Task<IActionResult> Page_WebsiteInformation_View() {
			var setting = await GetSetting();
			return View(setting);
		}

		// Page thông tin website (view)
		[Authorize]
		public async Task<IActionResult> Page_WebsiteInformation_Edit() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var setting = await context.TblSettings.FirstOrDefaultAsync();
				if (setting == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				var dataBase64s = new List<string>();
				var dataTypes = new List<string>();

				dataBase64s.Add(setting.LogoImg);
				dataBase64s.Add(setting.AuthImg);
				dataBase64s.Add(setting.FooterBgImg);
				for (int i = 0; i < 3; i++) {
					dataTypes.Add(string.Empty);
				}

				ViewData["DataBase64s"] = dataBase64s;
				ViewData["DataTypes"] = dataTypes;

				Helper.Validate.Clear();
				return View(setting);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access edit status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_WebsiteInformation_View");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_WebsiteInformation_Edit(TblSetting model, List<string> dataBase64s, List<string> dataTypes) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["DataTypes"] = dataTypes;

				// Begin validate
				Helper.Validate.Clear();
				int index = 0; // Check logo
				dataBase64s[index] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[index], dataTypes[index]);
				Helper.Validate.NotEmpty(dataBase64s[index]);
				
				index += 1; // Check auth img
				dataBase64s[index] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[index], dataTypes[index]);
				Helper.Validate.NotEmpty(dataBase64s[index]);

				Helper.Validate.MaxLength(model.SchoolNameEn, 256);
				Helper.Validate.MaxLength(model.SchoolNameVi, 256);
				Helper.Validate.MaxLength(model.FacultyEn, 256);
				Helper.Validate.MaxLength(model.FacultyVi, 256);

				index += 1; // check cho background footer và footer
				dataBase64s[index] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[index], dataTypes[index]);
				Helper.Validate.NotEmpty(dataBase64s[index]);

				Helper.Validate.NotEmpty(model.FooterDark.ToString());
				Helper.Validate.MaxLength(model.Phone, 20);
				Helper.Validate.MaxLength(model.Email, 256);
				Helper.Validate.MaxLength(model.AddressEn, 256);
				Helper.Validate.MaxLength(model.AddressVi, 256);
				Helper.Validate.NotEmpty(model.DescriptionWebsiteEn);
				Helper.Validate.NotEmpty(model.DescriptionWebsiteVi);

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Save data
				// Với logo
				index = 0;
				if (dataBase64s[index].Contains(Helper.Key.temp)) {
					Helper.Func.DeletePhoto(Helper.Path.logo, model.LogoImg);
					string fileName = $"logo.ico";
					model.LogoImg = fileName;
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[index]),
						Path.Combine(Helper.Path.logo, fileName)
					);
				}

				// Với auth 
				index += 1;
				if (dataBase64s[index].Contains(Helper.Key.temp)) {
					Helper.Func.DeletePhoto(Helper.Path.logo, model.AuthImg);
					string fileName = $"auth_img{Helper.Func.GetTypeImage(dataTypes[index])}";
					model.AuthImg = fileName;
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[index]),
						Path.Combine(Helper.Path.logo, fileName)
					);
				}

				// Với background footer
				index += 1;
				if (dataBase64s[index].Contains(Helper.Key.temp)) {
					Helper.Func.DeletePhoto(Helper.Path.logo, model.FooterBgImg);
					string fileName = $"bg-footer{Helper.Func.GetTypeImage(dataTypes[index])}";
					model.FooterBgImg = fileName;
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[index]),
						Path.Combine(Helper.Path.logo, fileName)
					);
				}

				context.TblSettings.Update(model);
				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo thành công 
				Helper.Notifier.Success(
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return RedirectToAction("Page_WebsiteInformation_View");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Edit request failed. Please try again later !" : "Yêu cầu chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View(model);
			}
		}
	}
}
