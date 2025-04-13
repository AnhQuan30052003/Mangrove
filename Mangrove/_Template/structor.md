// try...catch handle C# code

try {

}
catch (Exception ex) {
    string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
    Console.WriteLine(notifier);
    return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
}


// try...catch show error request

try {

}
catch {
    Helper.Notifier.Fail(
	    isEN ? "" : "",
	    Helper.SetupNotifier.Timer.midTime
    );
    return Content(Helper.Link.ScriptGetUrlBack(), "text/html");
}
