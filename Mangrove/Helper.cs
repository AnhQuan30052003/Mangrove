using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
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
	}

	// Status noifier
	public static class Key {
		public static string status = "Status";
		public static string timer = "Timer";
		public static string content = "Content";
		public static string toPage = "ToPage";
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

			Console.Clear();
			Console.WriteLine("Đã setup thông báo xong");
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

		// Format number
		public static string FormatNumber(long number) {
			string textNumber = number.ToString();
			if (textNumber.Length > 3) {
				textNumber = textNumber.Substring(0, 1) + "." + textNumber.Substring(1, 3);
			}
			return textNumber;
		}
	}
}