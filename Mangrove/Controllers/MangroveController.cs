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

		public async Task<IActionResult> Page_Index(string search = "", int currentPage = 1, int? pageSize = null) {
			try {
				if (pageSize == null) pageSize = InfomationPaginate.ListPageSize[0];

				string findText = search;
				ViewData["Search"] = search;

				bool isEN = Helper.Func.IsLanguage("EN");
				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				var data = await context.TblMangroves.ToListAsync();
				if (data.Count() == 0) data = new List<TblMangrove>();

				List<TblMangrove> fillter = data;
				fillter = new List<TblMangrove>();

				// Xử lý logic tìm kiếm
				foreach (var item in data) {
					bool check = Helper.Func.CheckContain(
						search,
						new List<string>() { 
							item.NameVi, item.NameEn,
							item.CommonNameVi, item.CommonNameEn, 
							item.ScientificName, item.Familia,
							item.DistributionVi, item.DistributionEn 
						}
					);

					if (check) {
						fillter.Add(item);
					}
				}

				var pagi = new PaginateModel<TblMangrove>(currentPage, (int)pageSize, fillter, "Both_PaginateTable_IndexMangrove", listTitle, findText, "Mangrove", "Page_Index");

				// Code Ajax tìm cây
				if (Request.Headers["REQUESTED"] == "AJAX") {
					return PartialView($"{Helper.Path.partialView}/{pagi.InfomationPaginate.NameTable}.cshtml", pagi);
				}

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
