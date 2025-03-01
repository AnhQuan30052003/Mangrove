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
    public static class Variable {
    }


    // Function
    public static class Func {
        public static string Show(string text) {
            return text.Replace("\n", "<br>");
        }
    }

    // public static class GoogleTranslateService {
    //     private static readonly TranslationClient _client = TranslationClient.CreateFromApiKey("AIzaSyA4AirVbB-1djs6087vMMFPtN-3ug08iJU");

    //     public static async Task<string> TranslateTextAsync(string text, string targetLanguage) {
    //         var response = await _client.TranslateTextAsync(text, targetLanguage);
    //         return response.TranslatedText;
    //     }
    // }
}