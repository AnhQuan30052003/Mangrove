// try...catch handle C# code

try {

}
catch (Exception ex) {
    string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
    Console.WriteLine(notifier);
    return NotFound(notifier);
}


// try...catch show error request

try {

}
catch {
    Helper.Notifier.Fail(
	    isEN ? "" : "",
	    Helper.SetupNotifier.Timer.midTime
    );
    return Content(Helper.Link.GetUrlBack(), "text/html");
}