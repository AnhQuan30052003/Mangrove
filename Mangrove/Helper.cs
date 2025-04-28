using Microsoft.CodeAnalysis.Elfie.Model;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Net.Mail;
using System.Net;
using QRCoder;
using Microsoft.Identity.Client;
using Mangrove.Data;
using Mangrove.Controllers;

public class Helper {
	private static IHttpContextAccessor? httpContextAccessor;

	public static void Configure(IHttpContextAccessor _httpContextAccessor) {
		httpContextAccessor = _httpContextAccessor;
	}

	// Path layout...
	public static class Path {
		public static string layoutUser = "~/Views/Shared/_LayoutUser.cshtml";
		public static string layoutAdmin = "~/Views/Shared/_LayoutAdmin.cshtml";
		public static string layoutAdmin_Auth = "~/Views/Shared/_LayoutAuth.cshtml";
		public static string partialView = "~/Views/_PartialView";
		public static string partialViewLayout = "~/Views/Shared/_PartialView_Layout";

		public static string logo = "wwwroot/img/logo";
		public static string treeImg = "wwwroot/img/tree-img";
		public static string stageImg = "wwwroot/img/stage-img";
		public static string qrImg = "wwwroot/img/qr-img";
		public static string distributionImg = "wwwroot/img/distribution-map-img";
		public static string temptImg = "wwwroot/img/temp-img";
		public static string sponImg = "wwwroot/img/spon-img";
		public static string overviewImg = "wwwroot/img/overview-img";
	}

	// Variable
	public static class Variable {
		public static int maxItem = 10;
		public static int maxItemImageIndividual = 20;
		public static int maxStage = 5;

		public static double timeSession = 365;
		public static double timeLogin = 365;

		public static string cookieName = "ASP_Auth_Mangrove";

		public class TypeError {
			public static string notPermission = "notPermission";
			public static string disconnectDatabase = "disconnectDatabase";
			public static string notExists = "notExists";
		}
	}

	// Links JS
	public static class Link {
		public static string local = "https://localhost:3005";
		public static string hosting = "https://mangrove.runasp.net";

		// Trở về link trước đó
		public static string ScriptGetUrlBack(string key, bool noScript = false, bool pageUser = false) {
			string urlRoot = pageUser ? Key.defaultUrlUser : Key.defaultUrlAdmin;
			string startScript = "<script>", endScript = "</script>";

			if (noScript) {
				startScript = endScript = string.Empty;
			}

			return @$"
				{startScript}
					const url = localStorage.getItem('{key}');
					if (url != null) {{
						location.href = url;
					}}
					else {{
						location.href = '{urlRoot}';
					}}
				{endScript}
			";
		}

		public static string JSSetUrlBack(string key) {
			return @$"
				const url = location.href;
				localStorage.setItem('{key}', url);
			";
		}
	}

	// Status noifier
	public static class Key {
		// For Notifier
		public static string status = "Status";
		public static string timer = "Timer";
		public static string content = "Content";
		public static string toPage = "ToPage";
		public static string sortASC = "asc";
		public static string sortDESC = "desc";
		public static string temp = "Temp";

		// For Link - Client
		public static string clientToPageResult = "clientToPageResult";
		public static string clientToPageIndividual = "clientToPageIndividual";
		public static string clientToPageSeachMangrove = "clientToPageSeachMangrove";
		public static string clientToPageDistribution = "clientToPageDistribution";

		// For Link - Admin
		public static string adminToPageChangePassword = "adminToPageChangePassword";
		public static string adminToPageInfo = "adminToPageInfo";
		public static string adminToPageSettingWebsite = "adminToPageSettingWebsite";
		public static string adminToPageHomePage = "adminToPageHomePage";
		public static string adminToPageOverviewMangrove = "adminToPageOverviewMangrove";

		public static string adminToPageListIndex = "adminToPageListIndex";
		public static string adminToPageDelete = "adminToPageDelete";

		// For Link - Default
		public static string defaultUrlAdmin = "/Admin/Page_Statistical";
		public static string defaultUrlUser = "/Home/Page_Index";

		// Cookie reset password
		public static string resetPassword = "resetPassword";
		public static string resetPasswordSave = "resetPasswordSave";

		// Key show item map
		public static string showGird = "showGrid";
		public static string showList = "showList";
	}

	// Setup noifier
	public static class SetupNotifier {
		public static class Status {
			public static string success = "success";
			public static string fail = "fail";
		}
		public static class Timer {
			public static int fastTime = 2000;
			public static int shortTime = 3000;
			public static int midTime = 7000;
			public static int longTime = 10000;
		}
	}

	// Nofifier
	public static class Notifier {
		public static void Create(string status, string content, int timer, string? toPage = null) {
			var context = httpContextAccessor?.HttpContext;
			if (context == null) return;

			context.Session.SetString(Key.status, status);
			context.Session.SetString(Key.content, content);
			context.Session.SetInt32(Key.timer, timer);
			context.Session.SetString(Key.toPage, toPage ?? string.Empty);
		}

		public static void Success(string content, int timer, string? toPage = null) => Create(SetupNotifier.Status.success, content, timer, toPage);
		public static void Fail(string content, int timer, string? toPage = null) => Create(SetupNotifier.Status.fail, content, timer, toPage);

		public static string GetStatus() {
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				return context.Session.GetString(Key.status) ?? string.Empty;
			}
			return string.Empty;
		}

		public static string GetContent() {
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				return context.Session.GetString(Key.content) ?? string.Empty;
			}
			return string.Empty;
		}

		public static int GetTimer() {
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				return context.Session.GetInt32(Key.timer) ?? 0;
			}
			return 0;
		}

		public static string GetToPage() {
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				return context.Session.GetString(Key.toPage) ?? string.Empty;
			}
			return string.Empty;
		}

		public static void Clear() {
			var context = httpContextAccessor?.HttpContext;
			if (context == null) return;

			context.Session.Remove(Key.status);
			context.Session.Remove(Key.content);
			context.Session.Remove(Key.timer);
			context.Session.Remove(Key.toPage);
		}
	}

	// Function
	public static class Func {
		// Hiển thị content với textare
		public static string Show(string text) {
			return text.Replace("\n", "<br>");
		}

		// Dịch Anh - Việt
		public static async Task<string> Translate(string input, string from = "vi", string to = "en") {
			try {
				int lenInput = input.Length;
				string translatedText = "";

				for (int i = 0, lenMaxTranslate = 1000; i < lenInput; i += lenMaxTranslate) {
					if (i + lenMaxTranslate > lenInput) lenMaxTranslate = lenInput - i;
					string text = input.Substring(i, lenMaxTranslate);
					string url = $"https://api.mymemory.translated.net/get?q={text}&langpair={from}|{to}";

					var httpClient = new HttpClient();
					var response = await httpClient.GetAsync(url);
					var jsonString = await response.Content.ReadAsStringAsync();

					using var doc = JsonDocument.Parse(jsonString);
					translatedText += doc.RootElement
						.GetProperty("responseData")
						.GetProperty("translatedText")
						.GetString() ?? text;
				}

				Console.WriteLine($"Done translated input: {input}");

				return translatedText;
			}
			catch (Exception ex) {
				Console.WriteLine($"Error: {ex.Message}");
				return input;
			}
		}

		// Chuỗi về Tiếng Việt không dấu
		public static string FormatUngisnedString(string input) {
			string normalized = input.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();

			foreach (char c in normalized) {
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) {
					sb.Append(c);
				}
			}

			string output = sb.ToString().Normalize(NormalizationForm.FormC).Replace("đ", "d").Replace("Đ", "D");
			return output;
		}

		// Lấy mã language hiện tại từ cookie
		public static string GetLanguageCurrent() {
			var context = httpContextAccessor?.HttpContext;
			if (context == null) {
				Console.WriteLine($"Context: null");
				return "en";
			}

			// Lấy cookie từ request
			string? lang = context.Request.Cookies[".AspNetCore.Culture"];
			return string.IsNullOrEmpty(lang) ? "en" : lang;
		}

		// Kiểm tra phải Tiếng Anh hay Việt không ?
		public static bool IsLanguage(string language) {
			return GetLanguageCurrent().Contains(language.ToLower());
		}

		// Kiểm tra có phải Tiếng Anh
		public static bool IsEnglish() => IsLanguage("en");

		// Format number
		public static string FormatNumber(long number) {
			string textNumber = number.ToString("N0").Replace(",", ".");
			return textNumber;
		}

		// Check exsits contain
		public static bool CheckContain(string key, List<string> data) {
			key = FormatUngisnedString(key.ToLower().Trim());

			foreach (string item in data) {
				string text = FormatUngisnedString(item.ToLower());
				if (text.Contains(key)) {
					return true;
				}
			}

			return false;
		}

		// Create id
		public static string CreateId() => Guid.NewGuid().ToString().ToUpper();

		// Save img với IFormFile
		public static async Task<string> SaveImage(string path, IFormFile? file) {
			if (file == null || file.Length == 0) return "";

			try {
				string extension = System.IO.Path.GetExtension(file.FileName);
				string fileName = $"{CreateId()}{extension}";
				string pathSave = System.IO.Path.Combine(path, fileName);

				using (var stream = new FileStream(pathSave, FileMode.Create)) {
					await file.CopyToAsync(stream);
					return fileName;
				}
			}
			catch {
				return "";
			}
		}

		// Save image from database 64
		public static async Task<bool> SaveImageFromBase64Data(string dataBase64, string folderSave, string fileName) {
			try {
				dataBase64 = dataBase64.Trim();

				// Kiểm tra nếu chuỗi có chứa tiền tố MIME
				if (dataBase64.StartsWith("data:image")) {
					int commaIndex = dataBase64.IndexOf(',');
					if (commaIndex > 0) {
						dataBase64 = dataBase64.Substring(commaIndex + 1);

						// Xóa khoảng trắng dư thừa
						dataBase64 = dataBase64.Replace("\n", "").Replace("\r", "");

						// Chuyển Base64 thành mảng byte
						byte[] imageBytes = Convert.FromBase64String(dataBase64);

						// Tạo thư mục lưu ảnh
						string imageName = fileName;
						string pathSave = System.IO.Path.Combine(folderSave, imageName);
						await File.WriteAllBytesAsync(pathSave, imageBytes);

						return true;
					}

					Console.WriteLine("Lỗi: không thấy ,");
					return false;
				}
				Console.WriteLine("Lỗi: không có data:image");
				return false;
			}
			catch {
				return false;
			}
		}

		// Xoá file ảnh
		public static void DeletePhoto(string folder, string fileName) {
			string path = System.IO.Path.Combine(folder, fileName);
			if (File.Exists(path)) {
				File.Delete(path);
			}
		}
		public static void DeletePhoto(string path) {
			if (File.Exists(path)) {
				File.Delete(path);
			}
		}

		// Lấy ID từ file ảnh
		public static string GetIdFromFileName(string fileName) {
			int index = fileName.IndexOf("_");
			if (index > 0) {
				return fileName.Substring(0, index);
			}
			return CreateId();
		}

		// Lấy đuôi file ảnh
		public static string GetTypeImage(string textType) {
			return $".{textType.Replace("image/", "").Replace("jpeg", "jpg")}";
		}

		// Xoá toàn bộ file trong thư mục chứa ảnh tạm
		public static void DeleteAllFile(string path) {
			if (System.IO.Path.Exists(path)) {
				foreach (var file in Directory.GetFiles(path)) {
					File.Delete(file);
				}
			}
		}

		// Chuyển ảnh từ thư mục tạm sang thư mục chính
		public static void MovePhoto(string oldPath, string newPath) {
			if (System.IO.Path.Exists(oldPath) && !System.IO.Path.Exists(newPath)) {
				File.Move(oldPath, newPath);
			}
		}

		// Kiểm tra xem có phải là chuỗi data base 64 không ?
		public static bool IsDataBase64String(string data) {
			return !string.IsNullOrEmpty(data) && data.StartsWith("data:image");
		}

		// Check dataBase64 và lưu
		public static async Task<string> CheckIsDataBase64StringAndSave(string data, string type) {
			if (IsDataBase64String(data)) {
				string fileTemp = $"{CreateId()}_{Key.temp}{GetTypeImage(type)}";
				bool status = await SaveImageFromBase64Data(
					data,
					Path.temptImg,
					fileTemp
				);

				if (status) return fileTemp;
				return data;
			}
			return data;
		}

		// Code generate 6 digit reset password
		public static string CreateCodeRandom(int length) {
			string code = string.Empty;
			Random random = new Random();
			for (int loop = 1; loop <= length; loop++) {
				code += random.Next(0, 10);
			}
			return code;
		}

		// Tạo mã QR
		public static string CreateQRCode(string url, string fileName) {
			var qrGenerator = new QRCodeGenerator();
			var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
			var pngQrCode = new PngByteQRCode(qrCodeData);
			byte[] qrCodeAsPng = pngQrCode.GetGraphic(20);

			// Lưu vào website
			string filePath = System.IO.Path.Combine(Path.qrImg, fileName);
			System.IO.File.WriteAllBytes(filePath, qrCodeAsPng);

			return fileName;
		}
	}

	// Validate
	public static class Validate {
		private static List<string> errors = new List<string>();
		private static int index = 0;

		// Add error
		public static void AddError(string err) => errors.Add(err);

		// Clear list errors
		public static void Clear() {
			errors.Clear();
			index = 0;
		}

		// Show error
		public static string ShowError() => index < errors.Count() ? errors[index++] : string.Empty;

		// Question have errors ?
		public static bool HaveError() {
			foreach (var err in errors) {
				if (!string.IsNullOrEmpty(err)) return true;
			}
			return false;
		}

		// Get list error
		public static List<string> GetListErrors() => errors;

		// Codes - functions check validate
		// Không rỗng
		public static void NotEmpty(string? value, bool allowNull = false) {
			string content = string.Empty;
			if (allowNull) {
				AddError(content);
				return;
			}

			if (string.IsNullOrEmpty(value)) {
				string EN = "Can't be blank !";
				string VI = "Không được bỏ trống !";
				content = Func.IsEnglish() ? EN : VI;
			}

			AddError(content);
		}

		// Không vượt qua x ký tự
		public static void MaxLength(string? value, int maxLength, bool allowNull = false) {
			if (value == null) {
				NotEmpty(value, allowNull);
				return;
			}

			string content = string.Empty;
			if (value.Length > maxLength) {
				string EN = $"No more than {maxLength} characters !";
				string VI = $"Không vượt quá {maxLength} ký tự !";
				content = Func.IsEnglish() ? EN : VI;
				AddError(content);
				return;
			}

			AddError(content);
		}

		// Có độ dài từ x -> y
		public static void TextLength(string? value, int minLength, int maxLength, bool allowNull = false) {
			if (string.IsNullOrEmpty(value)) {
				NotEmpty(value, allowNull);
				return;
			}

			string content = string.Empty;
			if (value.Length > maxLength) {
				MaxLength(value, maxLength, allowNull);
				return;
			}

			if (value.Length < minLength) {
				string EN = $"Length from {minLength} to {maxLength} characters !";
				string VI = $"Độ dài từ {minLength} đến {maxLength} ký tự !";
				content = Func.IsEnglish() ? EN : VI;
				AddError(content);
				return;
			}

			AddError(content);
		}

		// Có độ dài từ x ký tự
		public static void TextLength(string? value, int length, bool allowNull = false) {
			if (string.IsNullOrEmpty(value)) {
				NotEmpty(value, allowNull);
				return;
			}

			string content = string.Empty;
			if (value.Length == length) {
				AddError(content);
				return;
			}

			string EN = $"Must be {length} characters !";
			string VI = $"Phải có {length} ký tự !";
			content = Func.IsEnglish() ? EN : VI;
			AddError(content);
		}

		// Check email có hợp lệ
		public static void IsEmail(string? value, bool allowNull = false) {
			try {
				if (string.IsNullOrEmpty(value)) {
					NotEmpty(value, allowNull);
					return;
				}

				var email = new MailAddress(value);
				TextLength(value, 10, 256, allowNull);
			}
			catch {
				string EN = "Not a valid email !";
				string VI = "Không phải là email hợp lệ !";
				string content = Func.IsEnglish() ? EN : VI;
				AddError(content);
			}
		}
	}

	// Email
	public static class Email {
		// Lấy Form Html Reset Password
		public static string CreateFormHtmlResetPassword(string code) {
			bool isEN = Func.IsEnglish();

			string schoolName = isEN ? "Nha Trang University" : "Đại học Nha Trang";
			string instituteName = isEN ? "Institute of Biotechnology & Environment" : "Viện Công Nghệ Sinh Học & Môi Trường";
			string title = isEN ? "Password Recovery" : "Khôi phục mật khẩu";
			string hello = isEN ? "Hello Admin" : "Xin chào Quản trị viên";
			string resetLabel = isEN ? "your rebuild code is" : "mã tạo lại của bạn là";
			string remark = isEN ? "Remark" : "Lưu ý";
			string timeText = isEN ? "The value of the code is limited to" : "Giá trị của mã chỉ có thời hạn là";
			string time = isEN ? "5 minute" : "5 phút";

			string html = @$"
				<html>

				<head>
					<meta name='viewport' content='width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=5.0'>
				</head>

				<body style='overflow-x: hidden; margin: 0;'>
					<div class='mod_email'
						style='background-color: #eaeff1 !important; font-size: 18px; color: #707070; font-family: " + "'Oswald', sans-serif;'>" + @$"
						<div class='email_header' style='min-height: 60px; background-color: white; padding: 10px 5px; margin: 0;'>
							<div class='logo' style='display: inline; float: left;'>
								<img style='width: 60px; height: 60px;' src='{Link.hosting}/img/logo/logo.ico' alt=''>
							</div>

							<div class='info' style='display: inline; padding-left: 10px; float: left;'>
								<h3 style='font-weight: bold; padding: 3px 0; margin: 0;'>{schoolName}</h3>
								<p style='font-weight: bold; padding: 5px 0 10px 0; margin: 0;'>{instituteName}</p>
							</div>
						</div>

						<div class='email_body' style='padding: 10px; max-width: 500px; margin: 0 auto;'>
							<div class='email_title' style='text-align: center;'>
								<img style='width: 100px; height: 100px; margin: 30px 0;'
									src='{Link.hosting}/img/logo/password_recovery.png' alt=''>
								<h2 style='text-align: center; margin: 0;'>{title}</h2>
							</div>

							<div class='email_content' style='margin: 30px 0; text-align: center;'>
								<p style='display: inline-block; margin: 0 5px 10px 0;'>
									<b>{hello}</b>, {resetLabel}
								</p>
								<p
									style='display: inline-block; padding: 15px 20px; background-color: #f8f9fa; border-radius: 10px; margin: 0;'>
									{code}
								</p>
							</div>

							<div class='note'>
								<p style='color: red; text-decoration: underline; margin-bottom: 10px;'>{remark}:</p>
								<p>{timeText} <i style='text-decoration: underline;'>{time}</i>.</p>
							</div>
						</div>
					</div>
				</body>

				</html>
			";

			return html;
		}

		// Lấy Form Html Notifier Status Update Admin Information
		public static string CreateFormHtmlNotifierStatusAdminInformatoin(string username = "", string email = "", string codeRecret = "") {
			bool isEN = Func.IsEnglish();
			DateTime now = DateTime.Now;

			string schoolName = isEN ? "Nha Trang University" : "Đại học Nha Trang";
			string instituteName = isEN ? "Institute of Biotechnology & Environment" : "Viện Công Nghệ Sinh Học & Môi Trường";
			string title = isEN ? "Update admin information successfully" : "Cập nhật thông tin quản trị thành công";
			string hello = isEN ? "Hello Admin" : "Xin chào Quản trị viên";
			string labelHello = isEN ? $"on {now.ToString()} you updated admin information" : $"vào ngày {now.ToString()} bạn đã cập nhật thông tin quản trị viên";
			string keyLabel = isEN ? "Application key" : "Khoá ứng dụng";
			string usernameLabel = isEN ? "Username" : "Tên người dùng";

			string html = @$"
				<html>

				<head>
					<meta name='viewport' content='width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=5.0'>
				</head>

				<body style='overflow-x: hidden; margin: 0;'>
					<div class='mod_email'
						style='background-color: #eaeff1 !important; font-size: 18px; color: #707070; font-family: " + "'Oswald', sans-serif;'>" + $@"
						<div class='email_header' style='min-height: 60px; background-color: white; padding: 10px 5px; margin: 0;'>
							<div class='logo' style='display: inline; float: left;'>
								<img style='width: 60px; height: 60px;' src='{Link.hosting}/img/logo/logo.ico' alt=''>
							</div>

							<div class='info' style='display: inline; padding-left: 10px; float: left;'>
								<h3 style='font-weight: bold; padding: 3px 0; margin: 0;'>{schoolName}</h3>
								<p style='font-weight: bold; padding: 5px 0 10px 0; margin: 0;'>{instituteName}</p>
							</div>
						</div>

						<div class='email_body' style='padding: 10px; max-width: 600px; margin: 0 auto;'>
							<div class='email_title' style='text-align: center;'>
								<img style='width: 100px; height: 100px; margin: 30px 0;'
									src='{Link.hosting}/img/logo/check_success.png' alt=''>
								<h2 style='text-align: center; margin: 0;'>{title}</h2>
							</div>

							<div class='email_content' style='margin: 30px 0;'>
								<p style='line-height: 30px;'>
									<b>{hello}</b>, {labelHello}
								</p>

								<p>{usernameLabel}: <b><i>{username}</i></b></p>

								<p>Email: <b><i>{email}</i></b></p>

								<p>{keyLabel}:
									<span
										style='display: inline-block; padding: 10px; background-color: #f8f9fa; border-radius: 10px; margin: 0 5px;'>
										<b><i>{codeRecret.Trim()}</i></b>
									</span>
								</p>
							</div>
						</div>
					</div>
				</body>

				</html>
			";

			return html;
		}

		// Gửi email
		public static async void SendAsync(string email, string passwordCode, string toEmail, string subject, string body) {
			try {
				var smtpClient = new SmtpClient("smtp.gmail.com") {
					Port = 587,
					Credentials = new NetworkCredential(email, passwordCode),
					EnableSsl = true
				};

				var mailMessage = new MailMessage {
					From = new MailAddress(email),
					Subject = subject,
					Body = body,
					IsBodyHtml = true
				};

				mailMessage.To.Add(toEmail);
				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (Exception ex) {
				Console.WriteLine($"Error send email is: {ex.Message}");
			}
		}
	}
}