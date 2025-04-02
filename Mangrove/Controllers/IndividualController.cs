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

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
					{ listTitleVI[index++], item => item.IdMangroveNavigation!.NameVi },
					{ listTitleVI[index++], item => item.PositionVi },
					{ listTitleVI[index++], item => item.TblStages.Count() },
					{ listTitleVI[index++], item => item.UpdateLast },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
					{ listTitleEN[index++], item => item.IdMangroveNavigation!.NameEn },
					{ listTitleEN[index++], item => item.PositionEn },
					{ listTitleEN[index++], item => item.TblStages.Count() },
					{ listTitleEN[index++], item => item.UpdateLast },
				};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Tạo query
				var query = context.TblIndividuals
				.Include(item => item.TblStages)
				.Include(item => item.IdMangroveNavigation)
				.AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == "asc" ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();

				// Xử lý logic tìm kiếm
				List<TblIndividual> fillter = new List<TblIndividual>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.IdMangroveNavigation!.NameEn);
					conditions.Add(item.IdMangroveNavigation!.NameVi);
					conditions.Add(item.PositionEn);
					conditions.Add(item.PositionVi);
					conditions.Add(item.TblStages.Count().ToString());
					conditions.Add(item.UpdateLast.ToShortDateString());

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate(
					listTitle, currentPage, (int)pageSize, fillter.Count(),
					sortType, sortFollow, findText,
					"Individual", "Page_Index"
				);
				var pagi = new Paginate_VM<TblIndividual>(fillter, info);

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
			var mode = new TblIndividual();
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(TblDistributiton model, List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			return View();
		}

		// Edit
		public IActionResult Page_Edit() {
			return View();
		}

		// Detail
		public async Task<IActionResult> Page_Detail(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Truy vấn cá thể
				var individual = await context.TblIndividuals
				.Include(item => item.TblStages)
				.Include(item => item.IdMangroveNavigation)
				.FirstOrDefaultAsync(item => item.Id == id);
				if (individual == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				// Truy vấn giai đoạn và thông tin mỗi giai đoạn
				List<Stage> listStages = new List<Stage>();
				foreach (var stage in individual.TblStages.ToList()) {
					List<TblPhoto> photos = await context.TblPhotos.Where(item => item.IdObj == stage.Id).ToListAsync();
					var _stage = new Stage {
						info = stage,
						photo = photos
					};
					listStages.Add(_stage);
				}

				var info = new InfoStagesOfIndividual_Client_VM {
					IdIndividual = individual.Id,
					NameMangrove = isEN ? individual.IdMangroveNavigation!.NameEn : individual.IdMangroveNavigation!.NameVi + " - " + individual.IdMangroveNavigation!.ScientificName,
					Individual = individual,
					Stages = listStages
				};

				return View(info);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access individual page failed. Please try again later !" : "Gửi yêu cầu truy cập trang cá thể thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
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
