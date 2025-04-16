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
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Http;
using Humanizer;
using Microsoft.AspNetCore.Authorization;

namespace Mangrove.Controllers {
	[Authorize]
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
			try {
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
				var fileName = $"Statistical Top Mangrove_{DateTime.Now.ToString()}.xlsx";
				return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
			}
			catch {
				return NoContent();
			}
		}

		// Bộ lọc
		public async Task<IActionResult> Page_Fillter(
			DateTime? fromDate = null, DateTime? toDate = null, string? chooseData = null, bool searched = false,
			int pageSizeMangrove = 5, int currentPageMangrove = 1, string sortTypeMangrove = "asc", string? sortFollowMangrove = null,
			int pageSizeIndividual = 5, int currentPageIndividual = 1, string sortTypeIndividual = "asc", string? sortFollowIndividual = null) {

			fromDate = fromDate ?? DateTime.Today;
			toDate = toDate ?? DateTime.Today;

			// Save data
			ViewData["FromDate"] = fromDate;
			ViewData["ToDate"] = toDate;
			ViewData["ChooseData"] = chooseData;
			ViewData["Searched"] = searched;

			ViewData["PageSizeMangrove"] = pageSizeMangrove;
			ViewData["CurrentPageMangrove"] = currentPageMangrove;
			ViewData["SortTypeMangrove"] = sortTypeMangrove;
			ViewData["SortFollowMangrove"] = sortFollowMangrove;

			ViewData["PageSizeIndividual"] = pageSizeIndividual;
			ViewData["CurrentPageIndividual"] = currentPageIndividual;
			ViewData["SortTypeIndividual"] = sortTypeIndividual;
			ViewData["SortFollowIndividual"] = sortFollowIndividual;

			// Tăng thêm một ngày để quét tìm kiếm
			Paginate_VM<TblMangrove> pagiMangrove = await Get_Mangrove(
				fromDate, toDate.Value.AddDays(1),
				pageSizeMangrove, currentPageMangrove, sortTypeMangrove, sortFollowMangrove
			);

			Paginate_VM<TblIndividual> pagiIndividual = await Get_Individual(
				fromDate, toDate.Value.AddDays(1),
				pageSizeIndividual, currentPageIndividual, sortTypeIndividual, sortFollowIndividual
			);

			var model = new Statistical_Filter {
				ControllerName = "Statistical",
				ActionName = "Page_Fillter",
				Mangrove = pagiMangrove,
				Individual = pagiIndividual,
			};

			HttpContext.Session.SetString("fromDate", fromDate.ToString() ?? string.Empty);
			HttpContext.Session.SetString("toDate", toDate.ToString() ?? string.Empty);
			HttpContext.Session.SetString("sortTypeMangrove", sortTypeMangrove);
			HttpContext.Session.SetString("sortFollowMangrove", sortFollowMangrove ?? string.Empty);
			HttpContext.Session.SetString("sortTypeIndividual", sortTypeIndividual);
			HttpContext.Session.SetString("sortFollowIndividual", sortFollowIndividual ?? string.Empty);

			return View(model);
		}

		// Get_Mangrove
		private async Task<Paginate_VM<TblMangrove>> Get_Mangrove(DateTime? from = null, DateTime? to = null, int pageSize = 5, int currentPage = 1, string sortType = "asc", string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();

			// Setup cho bảng
			var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Hình thái", "Sinh thái", "Phân bố", "Tình trạng bảo tồn", "Công dụng", "Số lượng truy cập", "Số cá thể", "Chi tiết" };
			var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Morphology", "Ecology", "Distribution", "Conservation status", "Use", "Number of visits", "Number of individuals", "Detail" };
			var listTitle = isEN ? listTitleEN : listTitleVI;

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

		// Get_Individual
		private async Task<Paginate_VM<TblIndividual>> Get_Individual(DateTime? from = null, DateTime? to = null, int pageSize = 5, int currentPage = 1, string sortType = "asc", string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();

			// Setup cho bảng
			var listTitleVI = new List<string> { "STT", "Tên", "Vị trí", "Kinh độ", "Vĩ độ", "Số lượng truy cập", "Số giai đoạn", "Chi tiết" };
			var listTitleEN = new List<string> { "No", "Name", "Position", "Longitude", "Latitude", "Number of visits", "Number of stages", "Detail" };
			var listTitle = isEN ? listTitleEN : listTitleVI;

			int index = 1;
			var sortOptionsVI = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
			{
				{ listTitleVI[index++], item => item.IdMangroveNavigation!.NameVi },
				{ listTitleVI[index++], item => item.PositionVi },
				{ listTitleVI[index++], item => item.Longitude ?? string.Empty },
				{ listTitleVI[index++], item => item.Latitude ?? string.Empty },
				{ listTitleVI[index++], item => item.View },
				{ listTitleVI[index++], item => item.TblStages.Count() },
			};

			index = 1;
			var sortOptionsEN = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
			{
				{ listTitleEN[index++], item => item.IdMangroveNavigation!.NameVi },
				{ listTitleEN[index++], item => item.PositionVi },
				{ listTitleEN[index++], item => item.Longitude ?? string.Empty },
				{ listTitleEN[index++], item => item.Latitude ?? string.Empty },
				{ listTitleEN[index++], item => item.View },
				{ listTitleEN[index++], item => item.TblStages.Count() },
				{ listTitleEN[index++], item => item.TblStages.Count() },
			};
			var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

			// Tạo query
			var query = context.TblIndividuals
			.Include(item => item.TblStages)
			.Include(item => item.IdMangroveNavigation)
			.Where(item => item.UpdateLast >= from && item.UpdateLast <= to)
			.AsQueryable();

			// Kiểm tra nếu có thuộc tính cần sắp xếp
			if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
				var sortExpression = sortOptions[sortFollow];
				query = sortType == Helper.Key.sortASC ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
			}

			var data = await query.ToListAsync();

			ViewData["TotalData_Individual"] = data.Count();

			var info = new InfomationPaginate(
				listTitle, currentPage, (int)pageSize, data.Count(),
				sortType, sortFollow, string.Empty,
				string.Empty, string.Empty
			);
			var pagi = new Paginate_VM<TblIndividual>(data, info);

			return pagi;
		}

		// Xuất Excel (mangrove)
		public async Task<IActionResult> ExportToExcel_Mangrove() {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var from = Convert.ToDateTime(HttpContext.Session.GetString("fromDate"));
				var to = Convert.ToDateTime(HttpContext.Session.GetString("toDate")).AddDays(1);
				var sortType = HttpContext.Session.GetString("sortTypeMangrove");
				var sortFollow = HttpContext.Session.GetString("sortFollowMangrove");

				// Cấu hình EPPlus
				using var workbook = new XLWorkbook();
				var worksheet = workbook.Worksheets.Add("Excel");

				// Setup cho tiêu đề
				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Hình thái", "Sinh thái", "Phân bố", "Tình trạng bảo tồn", "Công dụng", "Số lượng truy cập", "Số cá thể", "Chi tiết" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Morphology", "Ecology", "Distribution", "Conservation status", "Use", "Number of visits", "Number of individuals", "Detail" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

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

				// Tạo tiêu đề
				for (int i = 0; i < listTitle.Count(); i++) {
					var title = listTitle[i];
					worksheet.Cell(1, 1 + i).Value = title;
				}

				// Ghi dữ liệu
				for (int i = 0; i < data.Count(); i++) {
					var item = data[i];
					worksheet.Cell(i + 2, 1).Value = i + 1;
					worksheet.Cell(i + 2, 2).Value = isEN ? item.NameEn : item.NameVi;
					worksheet.Cell(i + 2, 3).Value = isEN ? item.CommonNameEn : item.CommonNameVi;
					worksheet.Cell(i + 2, 4).Value = item.ScientificName;
					worksheet.Cell(i + 2, 5).Value = item.Familia;
					worksheet.Cell(i + 2, 6).Value = isEN ? item.MorphologyEn : item.MorphologyVi;
					worksheet.Cell(i + 2, 7).Value = isEN ? item.EcologyEn : item.EcologyVi;
					worksheet.Cell(i + 2, 8).Value = isEN ? item.DistributionEn : item.DistributionVi;
					worksheet.Cell(i + 2, 9).Value = isEN ? item.ConservationStatusEn : item.ConservationStatusVi;
					worksheet.Cell(i + 2, 10).Value = isEN ? item.UseEn : item.UseVi;
					worksheet.Cell(i + 2, 11).Value = item.View;
					worksheet.Cell(i + 2, 12).Value = item.TblIndividuals.Count();

					string link = $"https://mangrove.runasp.net/Home/Page_Result/{item.Id}";
					worksheet.Cell(i + 2, 13).Value = link;
					worksheet.Cell(i + 2, 13).SetHyperlink(new XLHyperlink(link));
					worksheet.Cell(i + 2, 13).Style.Font.Underline = XLFontUnderlineValues.Single;
					worksheet.Cell(i + 2, 13).Style.Font.FontColor = XLColor.Blue;
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
				var fileName = $"Statistical Mangrove_{DateTime.Now.ToString()}.xlsx";
				return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
			}
			catch {
				return NoContent();
			}
		}

		// Xuất Excel (individual)
		public async Task<IActionResult> ExportToExcel_Individual() {
			try {
				bool isEN = Helper.Func.IsEnglish();

				// Cấu hình EPPlus
				using var workbook = new XLWorkbook();
				var worksheet = workbook.Worksheets.Add("Excel");

				var from = Convert.ToDateTime(HttpContext.Session.GetString("fromDate"));
				var to = Convert.ToDateTime(HttpContext.Session.GetString("toDate")).AddDays(1);
				var sortType = HttpContext.Session.GetString("sortTypeIndividual");
				var sortFollow = HttpContext.Session.GetString("sortFollowIndividual");

				// Setup cho bảng
				var listTitleVI = new List<string> { "STT", "Tên", "Vị trí", "Kinh độ", "Vĩ độ", "Số lượng truy cập", "Số giai đoạn", "Chi tiết" };
				var listTitleEN = new List<string> { "No", "Name", "Position", "Longitude", "Latitude", "Number of visits", "Number of stages", "Detail" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
				{ listTitleVI[index++], item => item.IdMangroveNavigation!.NameVi },
				{ listTitleVI[index++], item => item.PositionVi },
				{ listTitleVI[index++], item => item.Longitude ?? string.Empty },
				{ listTitleVI[index++], item => item.Latitude ?? string.Empty },
				{ listTitleVI[index++], item => item.View },
				{ listTitleVI[index++], item => item.TblStages.Count() },
			};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
				{ listTitleEN[index++], item => item.IdMangroveNavigation!.NameVi },
				{ listTitleEN[index++], item => item.PositionVi },
				{ listTitleEN[index++], item => item.Longitude ?? string.Empty },
				{ listTitleEN[index++], item => item.Latitude ?? string.Empty },
				{ listTitleEN[index++], item => item.View },
				{ listTitleEN[index++], item => item.TblStages.Count() },
				{ listTitleEN[index++], item => item.TblStages.Count() },
			};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Tạo query
				var query = context.TblIndividuals
				.Include(item => item.TblStages)
				.Include(item => item.IdMangroveNavigation)
				.Where(item => item.UpdateLast >= from && item.UpdateLast <= to)
				.AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == Helper.Key.sortASC ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();

				// Tạo tiêu đề
				for (int i = 0; i < listTitle.Count(); i++) {
					var title = listTitle[i];
					worksheet.Cell(1, 1 + i).Value = title;
				}

				// Ghi dữ liệu
				for (int i = 0; i < data.Count(); i++) {
					var item = data[i];
					worksheet.Cell(i + 2, 1).Value = i + 1;
					worksheet.Cell(i + 2, 2).Value = isEN ? item.IdMangroveNavigation!.NameVi : item.IdMangroveNavigation!.NameVi;
					worksheet.Cell(i + 2, 3).Value = isEN ? item.PositionEn : item.PositionVi;
					worksheet.Cell(i + 2, 4).Value = item.Longitude;
					worksheet.Cell(i + 2, 5).Value = item.Latitude;
					worksheet.Cell(i + 2, 6).Value = item.View;
					worksheet.Cell(i + 2, 7).Value = item.TblStages.Count();

					string link = $"https://mangrove.runasp.net/Home/Page_Individual/{item.Id}";
					worksheet.Cell(i + 2, 8).Value = link;
					worksheet.Cell(i + 2, 8).SetHyperlink(new XLHyperlink(link));
					worksheet.Cell(i + 2, 8).Style.Font.Underline = XLFontUnderlineValues.Single;
					worksheet.Cell(i + 2, 8).Style.Font.FontColor = XLColor.Blue;
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
				var fileName = $"Statistical Individual_{DateTime.Now.ToString()}.xlsx";
				return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
			}
			catch {
				return NoContent();
			}
		}
	}
}
