using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Mangrove.Controllers {
	public class StatisticalController : Controller {
		private readonly MangroveContext context;

		public StatisticalController(MangroveContext context) {
			this.context = context;
		}

		// Tổng quan
		public async Task<IActionResult> Page_Overview(string? sortType = null, string? sortFollow = null) {
			// Lấy item cho tổng số lượng
			ViewData["CountMangrove"] = await context.TblMangroves.CountAsync();
			ViewData["CountIndividual"] = await context.TblIndividuals.CountAsync();
			ViewData["CountDistributionMap"] = await context.TblDistributitons.CountAsync();

			// Lấy top mangrove
			ViewData["TopMangrove"] = await Get_TopMangrove(sortType, sortFollow);
			HttpContext.Session.SetString("sortType", sortType ?? string.Empty);
			HttpContext.Session.SetString("sortFollow", sortFollow ?? string.Empty);

			return View();
		}

		// Get Top mangrove
		private async Task<List<TblMangrove>> Get_TopMangrove(string? sortType = null, string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();

			// Setup cho bảng
			var listTitleVI = new List<string> { "STT", "Tên", "Họ", "Số lượng truy cập", "Số cá thể", "Liên kết" };
			var listTitleEN = new List<string> { "No", "Name", "Familia", "Number of visits", "Number of individuals", "Link" };
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

			return topMangrove;
			;
		}

		// Xuất Excel (Top mangrove)
		public async Task<IActionResult> ExportToExcel() {
			bool isEN = Helper.Func.IsEnglish();

			// Cấu hình EPPlus
			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Excel");

			// Setup cho tiêu đề
			var listTitleVI = new List<string> { "STT", "Tên", "Họ", "Số lượng truy cập", "Số cá thể", "Liên kết" };
			var listTitleEN = new List<string> { "No", "Name", "Familia", "Number of visits", "Number of individuals", "Link" };
			var listTitle = isEN ? listTitleEN : listTitleVI;

			// Tạo tiêu đề
			for (int i = 0; i < listTitle.Count(); i++) {
				var title = listTitle[i];
				worksheet.Cell(1, 1 + i).Value = title;
			}

			// Ghi dữ liệu
			var sortType = HttpContext.Session.GetString("sortType");
			var sortFollow = HttpContext.Session.GetString("sortFollow");
			var data = await Get_TopMangrove(sortType, sortFollow);
			for (int i = 0; i < data.Count(); i++) {
				var item = data[i];
				worksheet.Cell(i + 2, 1).Value = i + 1;
				worksheet.Cell(i + 2, 2).Value = isEN ? item.NameEn : item.NameVi;
				worksheet.Cell(i + 2, 3).Value = item.Familia;
				worksheet.Cell(i + 2, 4).Value = item.View;
				worksheet.Cell(i + 2, 5).Value = item.TblIndividuals.Count();

				string link = $"https://mangrove.runasp.net/Home/Page_Result/{item.Id}";
				worksheet.Cell(i + 2, 6).Value = link;
				worksheet.Cell(i + 2, 6).SetHyperlink(new XLHyperlink(link));
				worksheet.Cell(i + 2, 6).Style.Font.Underline = XLFontUnderlineValues.Single;
				worksheet.Cell(i + 2, 6).Style.Font.FontColor = XLColor.Blue;
			}

			// Định dạng header: font đậm, màu nền, căn giữa
			var headerRow = worksheet.Row(1);
			headerRow.Style.Font.Bold = true;
			headerRow.Style.Fill.SetBackgroundColor(XLColor.LightGray);
			headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
			headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

			// Định dạng các ô dữ liệu
			var dataRange = worksheet.Range(2, 1, data.Count() + 1, listTitle.Count());
			dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

			// Tự động điều chỉnh chiều rộng cột
			worksheet.Columns().AdjustToContents();

			// Xuất file
			var stream = new MemoryStream();
			workbook.SaveAs(stream);
			stream.Seek(0, SeekOrigin.Begin);
			var fileName = $"Top Mangrove_{DateTime.Now.ToString()}.xlsx";
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

		// Bộ lọc
		public IActionResult Page_Fillter() {
			return View();
		}
	}
}
