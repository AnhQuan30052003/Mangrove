using System.Globalization;
using System.Text;
using System.Text.Json;

public class Helper {
    // Path layout...
    public static class Path {
        public static string layoutUser = "~/Views/Shared/_LayoutUser.cshtml";
        public static string layoutAdmin = "~/Views/Shared/_LayoutAdmin.cshtml";
        public static string layoutAdmin_Login = "~/Views/Shared/_LayoutAuth.cshtml";
        public static string partialView = "~/Views/_PartialView";
        public static string partialViewLayout = "~/Views/Shared/_PartialView_Layout";
    }

    // Status noifier
    public static class StatusNoifier {
        public static string success = "success";
        public static string fail = "fail";
    }

    // Variables
    public static class Variable { }

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
    }
}