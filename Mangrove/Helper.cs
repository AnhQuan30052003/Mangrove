using System.Globalization;
using System.Text;
using System.Text.Json;

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

		public static string treeImg = "wwwroot/img/tree-img";
		public static string stageImg = "wwwroot/img/stage-img";
		public static string qrImg = "wwwroot/img/qr-img";
		public static string distributionImg = "wwwroot/img/distribution-map-img";
		public static string temptImg = "wwwroot/img/temp-img";
	}

	// Variable
	public static class Variable {
		public static int maxItem = 10;
	}

	// Links JS
	public static class Link {
		// Trở về link trước đó
		public static string GetUrlBack(string? key = null) {
			string keyDefault = "urlIndex";
			if (key == null) {
				key = keyDefault;

				return @$"
					<script>
						const url = localStorage.getItem('{key}');
						if (url != null) {{
							location.href = url;
						}}
					</script>
				";
			}

			return @$"
				<script>
					let url = localStorage.getItem('{key}');
					if (url != null) {{
						location.href = url;
						localStorage.removeItem('{key}');
					}}
					else {{
						url = localStorage.getItem('{keyDefault}')
						if (url != null) location.href = url;
					}}
				</script>
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
		public static string notPhoto = "NotPhoto";
		public static string temp = "Temp";

		// For Link
		public static string urlBack = "urlIndex";
		public static string afterEdit = "afterEdit";
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
		public static void Create(string status, string content, int timer, string toPage = "") {
			var context = httpContextAccessor?.HttpContext;
			if (context == null) return;

			context.Response.Cookies.Append(Key.status, status);
			context.Response.Cookies.Append(Key.content, content);
			context.Response.Cookies.Append(Key.timer, timer.ToString());
			context.Response.Cookies.Append(Key.toPage, toPage);
		}

		public static string GetStatus() {
			var result = "";
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				result = context.Request.Cookies[Key.status] ?? result;
			}
			return result;
		}

		public static string GetContent() {
			var result = "";
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				result = context.Request.Cookies[Key.content] ?? result;
			}
			return result;
		}

		public static int GetTimer() {
			var result = SetupNotifier.Timer.longTime;
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				result = Convert.ToInt32(context.Request.Cookies[Key.timer]);
			}
			return result;
		}

		public static string GetToPage() {
			var result = "";
			var context = httpContextAccessor?.HttpContext;
			if (context != null) {
				result = context.Request.Cookies[Key.toPage] ?? result;
			}
			return result;
		}

		public static void Clear() {
			var context = httpContextAccessor?.HttpContext;
			if (context == null) return;

			context.Response.Cookies.Delete(Key.status);
			context.Response.Cookies.Delete(Key.content);
			context.Response.Cookies.Delete(Key.timer);
			context.Response.Cookies.Delete(Key.toPage);
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
			string textNumber = number.ToString();
			if (textNumber.Length > 3) {
				textNumber = textNumber.Substring(0, 1) + "." + textNumber.Substring(1, 3);
			}
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

		// Lấy đuôi file ảnh
		public static string GetTypeImage(string textType) {
			return textType.Replace("image/", "").Replace("jpeg", "jpg");
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
			if (System.IO.Path.Exists(oldPath)) {
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
				string fileTemp = $"{CreateId()}_{Key.temp}.{GetTypeImage(type)}";
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
		public static string ShowError() {
			try {
				return errors[index++];
			}
			catch {
				return "";
			}
		}

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
		public static void NotEmpty(string? text) {
			string content = string.Empty;
			if (string.IsNullOrEmpty(text)) {
				string EN = "Can't be blank !";
				string VI = "Không được bỏ trống !";
				content = Func.IsEnglish() ? EN : VI;

			}

			AddError(content);
		}
	}
}