using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	public class StatisticalController : Controller {
		private readonly MangroveContext context;

		public StatisticalController(MangroveContext context) {
			this.context = context;
		}

		// Tổng quan
		public async Task<IActionResult> Page_Overview(string? sortType = null, string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();

			// Save variables
			ViewData["SortType"] = sortType;
			ViewData["SortFollow"] = sortFollow;

			// Lấy item cho tổng số lượng
			ViewData["CountMangrove"] = await context.TblMangroves.CountAsync();
			ViewData["CountIndividual"] = await context.TblIndividuals.CountAsync();
			ViewData["CountDistributionMap"] = await context.TblDistributitons.CountAsync();

			// Setup cho bảng
			var listTitleVI = new List<string> { "STT", "Tên", "Họ", "Số lượng truy cập", "Số cá thể", "Liên kết"};
			var listTitleEN = new List<string> { "No", "Name", "Familia", "Number of visits", "Number of individuals", "Link"};
			var listTitle = isEN ? listTitleEN : listTitleVI;
			ViewData["ListTitle"] = listTitle;

			int index = 1;
			var sortOptionsVI = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
			{
				{ listTitleVI[index++], item => item.NameVi },
				{ listTitleVI[index++], item => item.Familia },
				{ listTitleVI[index++], item => item.View },
				{ listTitleVI[index++], item => item.TblIndividuals.Count() },
			};

			index = 1;
			var sortOptionsEN = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
			{
				{ listTitleEN[index++], item => item.NameEn },
				{ listTitleEN[index++], item => item.Familia },
				{ listTitleEN[index++], item => item.View },
				{ listTitleEN[index++], item => item.TblIndividuals.Count() },
			};
			var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

			// Tạo query
			var query = context.TblMangroves.Include(item => item.TblIndividuals).AsQueryable();

			// Kiểm tra nếu có thuộc tính cần sắp xếp
			if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
				var sortExpression = sortOptions[sortFollow];
				query = sortType == Helper.Key.sortASC ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
			}
			else {
				sortType = Helper.Key.sortDESC;
				sortFollow = listTitle[4];
				query = query.OrderByDescending(item => item.TblIndividuals.Count());
			}

			var topMangrove = await query
			.Take(10)
			.ToListAsync();

			ViewData["TopMangrove"] = topMangrove;

			return View();
		}

		// Bộ lọc
		public IActionResult Page_Fillter() {
			return View();
		}
	}
}
