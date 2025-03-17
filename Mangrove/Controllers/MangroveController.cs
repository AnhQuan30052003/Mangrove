using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
			this.context = context;
		}

		public async Task<IActionResult> Page_Index(string? search = null, int currentPage = 1, int? pageSize = null) {
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.ListPageSize[0];
				string findText = search ?? "";
				ViewData["Search"] = findText;

				bool isEN = Helper.Func.IsLanguage("EN");
				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				// Tạo query
				//var query = context.TblMangroves.AsQueryable();

				//if (!String.IsNullOrEmpty(sortType)) {
					
				//}

				//var sortOptions = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				//{
				//	{ "Name", isEN ? item => item.NameEn : item => item.NameVi },
				//	{ "CommonName", isEN ? item => item.CommonNameEn : item => item.CommonNameVi },
				//	{ "ScientificName", item => item.ScientificName },
				//	{ "Familia", item => item.Familia },
				//	{ "Distribution", isEN ? item => item.DistributionEn : item => item.DistributionVi }
				//};

				//// Kiểm tra nếu có thuộc tính cần sắp xếp
				//if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
				//	var sortExpression = sortOptions[sortFollow];

				//	query = sortType == "asc"
				//		? query.OrderBy(sortExpression)
				//		: query.OrderByDescending(sortExpression);
				//}

				var data = await context.TblMangroves.ToListAsync();
				if (data.Count() == 0) data = new List<TblMangrove>();

				// Test item
				//var temp = data;
				//data = new List<TblMangrove>();
				//for (int i = 1; i <= 100; i++) {
				//	foreach (var item in temp) {
				//		data.Add(item);
				//	}
				//}

				// Xử lý logic tìm kiếm
				List<TblMangrove> fillter = new List<TblMangrove>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.NameVi);
					conditions.Add(item.NameEn);
					conditions.Add(item.CommonNameVi);
					conditions.Add(item.CommonNameEn);
					conditions.Add(item.ScientificName);
					conditions.Add(item.Familia);
					conditions.Add(item.DistributionVi);
					conditions.Add(item.DistributionEn);

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate("Both_PaginateTable_IndexMangrove", listTitle, currentPage, (int) pageSize, fillter.Count(), findText, "Mangrove", "Page_Index");
				var pagi = new PaginateModel<TblMangrove>(fillter, info);

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
