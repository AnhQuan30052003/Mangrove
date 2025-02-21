// try...catch handle C# code

try {

}
catch (Exception ex) {
    Console.WriteLine("Error: " + ex.Message);
    return NotFound("Có lỗi khi kết nối với Cơ sở dữ liệu");
}