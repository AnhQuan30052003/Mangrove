using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

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
			var listTitleVI = new List<string> { "STT", "Tên", "Họ", "Số lượng truy cập", "Số cá thể", "Chi tiết" };
			var listTitleEN = new List<string> { "No", "Name", "Familia", "Number of visits", "Number of individuals", "Detail" };
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
		}

		// Xuất Excel (Top mangrove)
		public async Task<IActionResult> ExportToExcel_TopMangrove() {
			bool isEN = Helper.Func.IsEnglish();

			// Cấu hình EPPlus
			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Excel");

			// Setup cho tiêu đề
			var listTitleVI = new List<string> { "STT", "Tên", "Họ", "Số lượng truy cập", "Số cá thể", "Chi tiết" };
			var listTitleEN = new List<string> { "No", "Name", "Familia", "Number of visits", "Number of individuals", "Detail" };
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
		public async Task<IActionResult> Page_Fillter(
			DateTime? fromDate = null, DateTime? toDate = null, string? chooseData = null,
			int pageSizeMangrove = 5, int currentPageMangrove = 1, string sortTypeMangrove = "asc", string? sortFollowMangrove = null,
			int pageSizeIndividual = 5, int currentPageIndividual = 1, string sortTypeIndividual = "asc", string? sortFollowIndividual = null) {
			
			//fromDate = fromDate ?? new DateTime(2025, 01, 01);
			fromDate = fromDate ?? DateTime.Now;
			toDate = toDate ?? DateTime.Now;

			// Save data
			ViewData["FromDate"] = fromDate;
			ViewData["ToDate"] = toDate;
			ViewData["ChooseData"] = chooseData;

			ViewData["PageSizeMangrove"] = pageSizeMangrove;
			ViewData["CurrentPageMangrove"] = currentPageMangrove;
			ViewData["SortTypeMangrove"] = sortTypeMangrove;
			ViewData["SortFollowMangrove"] = sortFollowMangrove;

			ViewData["PageSizeIndividual"] = pageSizeIndividual;
			ViewData["CurrentPageIndividual"] = currentPageIndividual;
			ViewData["SortTypeIndividual"] = sortTypeIndividual;
			ViewData["SortFollowIndividual"] = sortFollowIndividual;

			Paginate_VM<TblMangrove> pagiMangrove = await Get_Mangrove(
				fromDate, toDate,
				pageSizeMangrove, currentPageMangrove, sortTypeMangrove, sortFollowMangrove
			);
			//Paginate_VM<TblIndividual> pagiIndividual = await Get_Individual(from, to, pageSize_I, currentPage_I, sortType_I, sortFollow_I);

			var model = new Statistical_Filter {
				ControllerName = "Statistical",
				ActionName = "Page_Fillter",
				Mangrove = pagiMangrove,
				Individual = null,
			};

			return View(model);
		}

		// Get Top mangrove
		private async Task<Paginate_VM<TblMangrove>> Get_Mangrove(DateTime? from = null, DateTime? to = null, int pageSize = 5, int currentPage = 1, string sortType = "asc", string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();

			// Setup cho bảng
			var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Hình thái", "Sinh thái", "Phân bố", "Tình trạng bảo tồn", "Công dụng", "Số lượng truy cập", "Số cá thể", "Chi tiết" };
			var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Morphology", "Ecology", "Distribution", "Conservation status", "Use", "Number of visits", "Number of individuals", "Detail" };
			var listTitle = isEN ? listTitleEN : listTitleVI;
			ViewData["ListTitle_Mangrove"] = listTitle;

			int index = 1;
			var sortOptionsVI = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
			{
				{ listTitleVI[index++], item => item.NameVi },
				{ listTitleVI[index++], item => item.CommonNameVi },
				{ listTitleVI[index++], item => item.ScientificName },
				{ listTitleVI[index++], item => item.Familia },
				{ listTitleVI[index++], item => item.MorphologyVi },
				{ listTitleVI[index++], item => item.EcologyVi },
				{ listTitleVI[index++], item => item.DistributionVi },
				{ listTitleVI[index++], item => item.ConservationStatusVi },
				{ listTitleVI[index++], item => item.UseVi },
				{ listTitleVI[index++], item => item.View },
				{ listTitleVI[index++], item => item.TblIndividuals.Count() },
			};

			index = 1;
			var sortOptionsEN = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
			{
				{ listTitleEN[index++], item => item.NameVi },
				{ listTitleEN[index++], item => item.CommonNameVi },
				{ listTitleEN[index++], item => item.ScientificName },
				{ listTitleEN[index++], item => item.Familia },
				{ listTitleEN[index++], item => item.MorphologyVi },
				{ listTitleEN[index++], item => item.EcologyVi },
				{ listTitleEN[index++], item => item.DistributionVi },
				{ listTitleEN[index++], item => item.ConservationStatusVi },
				{ listTitleEN[index++], item => item.UseVi },
				{ listTitleEN[index++], item => item.View },
				{ listTitleEN[index++], item => item.TblIndividuals.Count() },
			};
			var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

			// Tạo query
			var query = context.TblMangroves
			.Include(item => item.TblIndividuals)
			.Where(item => item.UpdateLast >= from && item.UpdateLast <= to)
			.AsQueryable();

			// Kiểm tra nếu có thuộc tính cần sắp xếp
			if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
				var sortExpression = sortOptions[sortFollow];
				query = sortType == Helper.Key.sortASC ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
			}

			var data = await query.ToListAsync();

			ViewData["TotalData_Mangrove"] = data.Count();

			var info = new InfomationPaginate(
				listTitle, currentPage, (int)pageSize, data.Count(),
				sortType, sortFollow, string.Empty,
				string.Empty, string.Empty
			);
			var pagi = new Paginate_VM<TblMangrove>(data, info);

			return pagi;
		}
	}
}
