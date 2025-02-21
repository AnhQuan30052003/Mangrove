// JavaScript

try
{

}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
    return NotFound("Không kết nối được với Cơ sở dữ liệu");
}