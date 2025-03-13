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
				var data = await context.TblMangroves.ToListAsync();

				bool isEN = Helper.Func.IsLanguage("EN");
				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				var pagi = new PaginateModel<TblMangrove>(currentPage, pageSize, data, listTitle, search, "Mangrove", "Index");
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
