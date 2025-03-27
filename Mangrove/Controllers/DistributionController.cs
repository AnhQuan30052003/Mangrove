using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.GetFistPageSize();
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				bool isEN = Helper.Func.IsEnglish();

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
			var models = new List<Distribution_VM>();
			return View(models);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(List<Distribution_VM> models) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validate
				Helper.Validate.Clear();
				for (int i = 0; i < models.Count(); i++) {
					var model = models[i];
					Helper.Validate.NotEmpty(model.Image.DataBase64);
					Helper.Validate.NotEmpty(model.MapNameEn);
					Helper.Validate.NotEmpty(model.MapNameVi);
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					return View(models);
				}

				// Khi không có ảnh nào
				if (!models.Any()) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "Must have at least one map !" : "Phải có ít nhất một bản đồ !",
						Helper.SetupNotifier.Timer.midTime,
						""
					);
					return RedirectToAction("Page_Create");
				}
				// End validate

				// Lưu bản đồ
				int itemSaveSuccess = 0;
				foreach (var model in models) {
					string id = Helper.Func.CreateId();
					string fileName = $"{id}_{model.MapNameVi}.{model.Image.DataType.Replace("image/", "").Replace("jpeg", "jpg")}";

					// Lưu ảnh
					bool statusSave = await Helper.Func.SaveImageFromBase64Data(
						model.Image.DataBase64,
						Helper.Path.distributionImg,
						fileName
					);

					// Nếu không lưu được thi bỏ qua
					if (!statusSave) continue;
					itemSaveSuccess += 1;

					// Tạo map để lưu vào database
					var map = new TblDistributiton {
						Id = id,
						ImageMap = fileName,
						MapNameEn = model.MapNameEn,
						MapNameVi = model.MapNameVi
					};
					context.TblDistributitons.Add(map);
				}
				await context.SaveChangesAsync();

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? $"Added {itemSaveSuccess} map." : $"Đã thêm {itemSaveSuccess} bản đồ.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);

				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch {
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? $"There was an error adding the map !" : $"Có lỗi trong quá trình thêm bản đồ !",
					Helper.SetupNotifier.Timer.midTime,
					""
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}

		// Chỉnh sửa
		public async Task<IActionResult> Page_Edit(string id) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var map = await context.TblDistributitons
				.Select(item => new Distribution_VM {
					Id = item.Id,
					ImageName = item.ImageMap,
					MapNameEn = item.MapNameEn,
					MapNameVi = item.MapNameVi,
					Image = new DataImage {
						DataType = "type",
						DataBase64 = "base64"
					}
				})
				.FirstOrDefaultAsync(item => item.Id == id);

				if (map == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "The edit page you just visited does not exist !" : "Trang chỉnh sửa vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.midTime
					);
					return RedirectToAction("Page_Index");
				}

				return View(map);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Edit(Distribution_VM model) {
			// Begin validate
			Helper.Validate.Clear();
			Helper.Validate.NotEmpty(model.Image.DataBase64);
			Helper.Validate.NotEmpty(model.MapNameEn);
			Helper.Validate.NotEmpty(model.MapNameVi);

			if (Helper.Validate.HaveError()) {
				return View(model);
			}
			// End validate

			bool isEN = Helper.Func.IsEnglish();
			var thisMap = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == model.Id);

			if (thisMap == null) {
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? $"There was an error editing the map !" : $"Có lỗi trong quá trình chỉnh sửa bản đồ !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}

			// Xử lý hình ảnh và dữ liệu
			// Nếu có ảnh mới
			string fileName = $"{thisMap.Id}_{model.MapNameVi}.{model.Image.DataType.Replace("image/", "").Replace("jpeg", "jpg")}";
			if (model.Image.DataBase64 != "base64") {
				bool statusSave = await Helper.Func.SaveImageFromBase64Data(
					model.Image.DataBase64,
					Helper.Path.distributionImg,
				fileName
				);

				if (statusSave && thisMap.ImageMap != fileName) {
					Helper.Func.DeletePhoto(Helper.Path.distributionImg, thisMap.ImageMap);
					thisMap.ImageMap = fileName;
				}
			}
			else {
				if (thisMap.ImageMap != fileName) {
					string extension = Path.GetExtension(thisMap.ImageMap);
					fileName = $"{thisMap.Id}_{model.MapNameVi}{extension}";
					string oldPath = Path.Combine(Helper.Path.distributionImg, thisMap.ImageMap);
					string newPath = Path.Combine(Helper.Path.distributionImg, fileName);

					if (System.IO.File.Exists(oldPath)) {
						System.IO.File.Move(oldPath, newPath);
						thisMap.ImageMap = fileName;
					}
				}
			}

			thisMap.MapNameEn = model.MapNameEn;
			thisMap.MapNameVi = model.MapNameVi;

			context.TblDistributitons.Update(thisMap);
			await context.SaveChangesAsync();

			// Setup thông báo
			Helper.Notifier.Create(
				Helper.SetupNotifier.Status.success,
				isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
				Helper.SetupNotifier.Timer.shortTime,
				""
			);
			return Content(Helper.Link.GetUrlBack(), "text/html");
		}

		// Chi tiết
		public async Task<IActionResult> Page_Detail(string id) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var map = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == id);
				if (map == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "The detail page you just visited does not exist !" : "Trang chi tiết vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.midTime
					);
					return RedirectToAction("Page_Index");
				}

				return View(map);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Xoá
		public async Task<IActionResult> Page_Delete(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Xoá đối tượng
				var map = await context.TblDistributitons.FirstOrDefaultAsync(item => item.Id == id);
				if (map == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "Map to delete not found !" : "Không tìm thấy bản đồ cần xoá !",
						Helper.SetupNotifier.Timer.midTime,
						""
					);
					return RedirectToAction("Page_Index");
				}

				context.TblDistributitons.Remove(map);
				await context.SaveChangesAsync();

				// Xoá ảnh
				Helper.Func.DeletePhoto(Helper.Path.distributionImg, map.ImageMap);

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Delete successfully." : "Đã xoá thành công.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);

				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch {
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? "Map delete failed !" : "Xoá bản đồ thất bại !",
					Helper.SetupNotifier.Timer.midTime,
					""
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}
	}
}
