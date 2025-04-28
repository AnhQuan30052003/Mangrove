using Microsoft.AspNetCore.Mvc;

namespace Mangrove.Controllers {
	public class CkEditorController : Controller {
		// Action xử lý upload ảnh
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
            {
                return Json(new { error = "Không có tệp ảnh được gửi." });
            }

            var uploadPath = Path.Combine(Helper.Path.overviewImg, upload.FileName);
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                await upload.CopyToAsync(fileStream);
            }

            // Trả về URL của ảnh đã tải lên để CKEditor sử dụng
            var fileUrl = $"/overview-img/{upload.FileName}";

            Console.Clear();
            Console.WriteLine($"File image: {fileUrl}");
            return Json(new { url = fileUrl });
        }
	}
}
