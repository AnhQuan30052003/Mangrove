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
        public static string Show(string text) {
            return text.Replace("\n", "<br>");
        }

        public static async Task<string> Translate(string text, string from = "vi", string to = "en") {
            try {
                string url = $"https://api.mymemory.translated.net/get?q={text}&langpair={from}|{to}";

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                var jsonString = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(jsonString);
                string translatedText = doc.RootElement
                    .GetProperty("responseData")
                    .GetProperty("translatedText")
                    .GetString() ?? text;

                return translatedText;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                return text;
            }
        }
    }
}