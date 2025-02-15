public class Helper {
    // Path layout...
    public static class Path {
        public static string layoutUser = "~/Views/Shared/_Layout_user.cshtml";
        public static string layoutAdmin = "";
        public static string components = "~/Views/Shared/Components";
        public static string _components = "~/Views/_Components";
    }

    // Status noifier
    public static class StatusNoifier {
        public static string success = "success";
        public static string fail = "fail";
	}

    // Variables
    public static class Variable {

    }

    // public static class GoogleTranslateService {
    //     private static readonly TranslationClient _client = TranslationClient.CreateFromApiKey("AIzaSyA4AirVbB-1djs6087vMMFPtN-3ug08iJU");

    //     public static async Task<string> TranslateTextAsync(string text, string targetLanguage) {
    //         var response = await _client.TranslateTextAsync(text, targetLanguage);
    //         return response.TranslatedText;
    //     }
    // }
}