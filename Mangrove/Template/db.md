// try...catch handle C# code

try {

}
catch (Exception ex) {
    string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
    Console.WriteLine(notifier);
    return NotFound(notifier);
}