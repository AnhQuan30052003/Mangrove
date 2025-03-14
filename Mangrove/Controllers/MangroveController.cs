using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
			this.context = context;
		}

		public async Task<IActionResult> Page_Index(string search = "", int currentPage = 1, int pageSize = 5) {
			try {
				string findText = search;
				ViewData["Search"] = search;
				bool isEN = Helper.Func.IsLanguage("EN");
				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				var data = await context.TblMangroves.ToListAsync();
				if (data.Count() == 0) data = new List<TblMangrove>();


				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {

					// Xử lý logic tìm kiếm
					List<TblMangrove> fillter = data;
					if (!string.IsNullOrEmpty(search)) {
						search = search.ToLower();
						string unsignStringSearch = Helper.Func.FormatUngisnedString(search);

						fillter = new List<TblMangrove>();
						foreach (var item in data) {
							if (item.NameVi.ToLower().Contains(search) || Helper.Func.FormatUngisnedString(item.NameVi.ToLower()).Contains(unsignStringSearch) || item.NameEn.ToLower().Contains(search)) {
								fillter.Add(item);
							}
						}
					}

					var pagiFillter = new PaginateModel<TblMangrove>(currentPage, pageSize, fillter, listTitle, findText, "Mangrove", "Page_Index");
					Console.WriteLine("Đã gửi Ajax: " + search);
					return PartialView($"{Helper.Path.partialView}/Both_PaginateBody_IndexMangrove.cshtml", pagiFillter);
				}

				var pagi = new PaginateModel<TblMangrove>(currentPage, pageSize, data, listTitle, findText, "Mangrove", "Page_Index");
				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
	}
}
