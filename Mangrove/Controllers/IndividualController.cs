using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	public class IndividualController : Controller {
		private readonly MangroveContext context;

		public IndividualController(MangroveContext context) {
			this.context = context;
		}

		// List
		public async Task<IActionResult> Page_Index(string? search = null, int currentPage = 1, int? pageSize = null, string? sortType = null, string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.GetFistPageSize();
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				var listTitleVI = new List<string> { "STT", "Tên cá thể", "Vị trí", "Số giai đoạn", "Cập nhật lần cuối", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Individual name", "Position", "Number of stage", "Last updated", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 0;
				var sortOptionsVI = new Dictionary<string, Expression<Func<Individual_ListIndex_Admin_VM, object>>>()
				{
					{ listTitleVI[index++], item => item.NameVI },
					{ listTitleVI[index++], item => item.PositionVI },
					{ listTitleVI[index++], item => item.TotalStage },
					{ listTitleVI[index++], item => item.UpdateLast },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<Individual_ListIndex_Admin_VM, object>>>()
				{
					{ listTitleEN[index++], item => item.NameEN },
					{ listTitleEN[index++], item => item.PositionEN },
					{ listTitleEN[index++], item => item.TotalStage },
					{ listTitleEN[index++], item => item.UpdateLast },
				};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Truy vấn NameEN, NameVI và ID mangrove
				var listMangrove = await context.TblMangroves.ToListAsync();
				var listNameVI_mangrove = new List<string>();
				var listNameEN_mangrove = new List<string>();
				var listID_mangrove = new List<string>();
				foreach (var mangrove in listMangrove) {
					listNameEN_mangrove.Add(mangrove.NameEn);
					listNameVI_mangrove.Add(mangrove.NameVi);
					listID_mangrove.Add(mangrove.Id);
				}

				// Lấy ra trường dữ liệu cần thiết
				var getQuery = await context.TblIndividuals.Include(item => item.TblStages).ToListAsync();

				// Tạo query
				var query = getQuery.Select(
					item => new Individual_ListIndex_Admin_VM {
						Id = item.Id,
						NameEN = listNameEN_mangrove[Helper.Func.FindIndex(listID_mangrove, item.IdMangrove ?? string.Empty)],
						NameVI = listNameVI_mangrove[Helper.Func.FindIndex(listID_mangrove, item.IdMangrove ?? string.Empty)],
						PositionEN = item.PositionEn,
						PositionVI = item.PositionVi,
						TotalStage = item.TblStages.Count(),
						UpdateLast = item.UpdateLast
					}
				).AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == "asc" ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();

				// Xử lý logic tìm kiếm
				List<Individual_ListIndex_Admin_VM> fillter = new List<Individual_ListIndex_Admin_VM>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.NameEN);
					conditions.Add(item.NameVI);
					conditions.Add(item.PositionEN);
					conditions.Add(item.PositionVI);
					conditions.Add(item.TotalStage.ToString());

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate(
					listTitle, currentPage, (int)pageSize, fillter.Count(),
					sortType, sortFollow, findText,
					"Individual", "Page_Index"
				);
				var pagi = new Paginate_VM<Individual_ListIndex_Admin_VM>(fillter, info);

				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
			}
		}

		// Create
		public IActionResult Page_Create() {
			return View();
		}

		// Edit
		public IActionResult Page_Edit() {
			return View();
		}

		// Detail
		public IActionResult Page_Detail() {
			return View();
		}

		// Delete
		public async Task<IActionResult> Page_Delete(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Xoá đối tượng
				var individual = await context.TblIndividuals.FirstOrDefaultAsync(item => item.Id == id);
				if (individual == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}
				context.TblIndividuals.Remove(individual);
				await context.SaveChangesAsync();

				// Xoá ảnh
				var individualPhotos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				if (individualPhotos.Any()) {
					foreach (var photo in individualPhotos) {
						context.TblPhotos.Remove(photo);
						Helper.Func.DeletePhoto(Helper.Path.stageImg, photo.ImageName);
					}
				}
				await context.SaveChangesAsync();

				// Setup thông báo
				Helper.Notifier.Success(
					isEN ? "Delete successfully." : "Đã xoá thành công.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageListIndex), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Delete request failed. Please try again later !" : "Yêu cầu xoá thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageListIndex), "text/html");
			}
		}
	}
}
