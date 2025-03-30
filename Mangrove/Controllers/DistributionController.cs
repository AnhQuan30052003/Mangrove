using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	public class DistributionController : Controller {
		private readonly MangroveContext context;

		public DistributionController(MangroveContext context) {
			this.context = context;
		}

		// Danh sách
		public async Task<IActionResult> Page_Index(string? search = null, int currentPage = 1, int? pageSize = null, string? sortType = null, string? sortFollow = null) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.GetFistPageSize();
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				var listTitleVI = new List<string> { "STT", "Vị trí", "Tên ảnh", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Position", "Photo name", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblDistributiton, object>>>()
				{
					{ listTitleVI[index++], item => item.MapNameVi },
					{ listTitleVI[index++], item => item.ImageMap },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblDistributiton, object>>>()
				{
					{ listTitleEN[index++], item => item.MapNameEn },
					{ listTitleEN[index++], item => item.ImageMap },

				};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Tạo query
				var query = context.TblDistributitons.AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == "asc" ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();

				// Xử lý logic tìm kiếm
				List<TblDistributiton> fillter = new List<TblDistributiton>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.MapNameEn);
					conditions.Add(item.MapNameVi);
					conditions.Add(item.ImageMap);

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate(listTitle, currentPage, (int)pageSize, fillter.Count(), sortType, sortFollow, findText, "Distribution", "Page_Index");
				var pagi = new Paginate_VM<TblDistributiton>(fillter, info);

				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Tạo mới
		public IActionResult Page_Create() {
			Helper.Validate.Clear();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validate
				Helper.Validate.Clear();

				for (int i = 0; i < dataBase64s.Count(); i++) {
					dataBase64s[i] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[i], dataTypes[i]);
					Helper.Validate.NotEmpty(dataBase64s[i]);
					Helper.Validate.NotEmpty(noteENs[i]);
					Helper.Validate.NotEmpty(noteVIs[i]);
				}

				TempData["DataTypes"] = dataTypes;
				TempData["DataBase64s"] = dataBase64s;
				TempData["NoteENs"] = noteENs;
				TempData["NoteVIs"] = noteVIs;

				// Khi không có ảnh nào
				if (!dataBase64s.Any()) {
					Helper.Notifier.Fail(
						isEN ? "Must have at least one map !" : "Phải có ít nhất một bản đồ !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Required fields are not filled in!" : "Các ô bắt buộc chưa được nhập !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View();
				}
				// End validate

				// Lưu dữ liệu
				for (int i = 0; i < dataTypes.Count(); i++) {
					string id = Helper.Func.CreateId();
					string fileName = $"{id}_{noteVIs[i]}{Helper.Func.GetTypeImage(dataTypes[i])}";

					// Chuyển ảnh vào đúng thư mục
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[i]),
						Path.Combine(Helper.Path.distributionImg, fileName)
					);

					// Tạo map để lưu vào database
					var map = new TblDistributiton {
						Id = id,
						ImageMap = fileName,
						MapNameEn = noteENs[i],
						MapNameVi = noteVIs[i]
					};
					context.TblDistributitons.Add(map);
				}

				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo
				Helper.Notifier.Success(
					isEN ? $"Added {dataBase64s.Count()} map." : $"Đã thêm {dataBase64s.Count()} bản đồ.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? $"TThere was an error adding the map. Please try again later !" : $"Có lỗi trong quá trình thêm bản đồ. Vui lòng thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}

		// Chỉnh sửa
		public async Task<IActionResult> Page_Edit(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var map = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == id);
				if (map == null) {
					Helper.Notifier.Fail(
						isEN ? "The edit page you just visited does not exist !" : "Trang chỉnh sửa vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return Content(Helper.Link.GetUrlBack(), "text/html");
				}

				TempData["DataBase64"] = map.ImageMap;
				Helper.Validate.Clear();
				return View(map);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access edit status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Edit(TblDistributiton model, string dataBase64, string dataType) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validate
				Helper.Validate.Clear();

				dataBase64 = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64, dataType);
				Helper.Validate.NotEmpty(dataBase64);
				Helper.Validate.NotEmpty(model.MapNameEn);
				Helper.Validate.NotEmpty(model.MapNameVi);

				TempData["DataBase64"] = dataBase64;

				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Required fields are not filled in!" : "Các ô bắt buộc chưa được nhập !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Xử lý hình ảnh và dữ liệu
				// Nếu có ảnh mới
				string fileName = $"{model.Id}_{model.MapNameVi}";
				string oldPath = Path.Combine(Helper.Path.distributionImg, model.ImageMap);
				if (dataBase64.Contains(Helper.Key.temp)) {
					Helper.Func.DeletePhoto(Helper.Path.distributionImg, model.ImageMap);
					fileName += Helper.Func.GetTypeImage(dataType);
					oldPath = Path.Combine(Helper.Path.temptImg, dataBase64);
				}
				else {
					fileName += Path.GetExtension(model.ImageMap);
				}

				model.ImageMap = fileName;
				string newPath = Path.Combine(Helper.Path.distributionImg, fileName);
				Helper.Func.MovePhoto(oldPath, newPath);

				context.TblDistributitons.Update(model);
				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo
				Helper.Notifier.Success(
				isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.GetUrlBack(Helper.Key.afterEdit), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Edit request failed. Please try again later !" : "Yêu cầu chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}

		// Chi tiết
		public async Task<IActionResult> Page_Detail(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var map = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == id);
				if (map == null) {
					Helper.Notifier.Fail(
						isEN ? "The detail page you just visited does not exist !" : "Trang chi tiết vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return Content(Helper.Link.GetUrlBack(), "text/html");
				}

				return View(map);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access detail status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chi tiết thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}

		// Xoá
		public async Task<IActionResult> Page_Delete(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Xoá đối tượng
				var map = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == id);
				if (map == null) {
					Helper.Notifier.Fail(
						isEN ? "Map to delete not found !" : "Không tìm thấy bản đồ cần xoá !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return RedirectToAction("Page_Index");
				}

				context.TblDistributitons.Remove(map);
				await context.SaveChangesAsync();

				// Xoá ảnh
				Helper.Func.DeletePhoto(Helper.Path.distributionImg, map.ImageMap);

				// Setup thông báo
				Helper.Notifier.Success(
					isEN ? "Delete successfully." : "Đã xoá thành công.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Delete request failed. Please try again later !" : "Yêu cầu xoá thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}
	}
}
