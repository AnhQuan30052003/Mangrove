using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
				if (pageSize == null) pageSize = InfomationPaginate.ListPageSize[0];
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				bool isEN = Helper.Func.IsLanguage("EN");

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
				//var data = await context.TblMangroves.ToListAsync();

				// Xử lý logic tìm kiếm
				List<TblDistributiton> fillter = new List<TblDistributiton>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.MapNameEn);
					conditions.Add(item.MapNameVi);
					conditions.Add(item.ImageMap);

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate("Both_PaginateTable_IndexDistribution", listTitle, currentPage, (int)pageSize, fillter.Count(), sortType, sortFollow, findText, "Distribution", "Page_Index");
				var pagi = new PaginateModel<TblDistributiton>(fillter, info);

				// Code Ajax tìm cây [bỏ]
				//if (Request.Headers["REQUESTED"] == "AJAX") {
				//	return PartialView($"{Helper.Path.partialView}/{pagi.InfomationPaginate.NameTable}.cshtml", pagi);
				//}

				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Tạo mới
		public IActionResult Call_Create() {
			var model = new DistributionModel();
			return RedirectToAction("Page_Create", model);
		}
		public IActionResult Page_Create(DistributionModel model) {

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(DistributionModel model, string? info = null) {
			bool isEN = Helper.Func.IsLanguage("en");

			// Bắt lỗi không có ảnh cho slide
			//if (!model.Photos.Any()) {
			//	Helper.Notifier.Create(
			//		Helper.SetupNotifier.Status.fail,
			//		isEN ? "Must have at least one photo!" : "Phải có ít nhất một ảnh !",
			//		Helper.SetupNotifier.Timer.shortTime,
			//		""
			//	);
			//	return RedirectToAction("Page_Create", model);
			//}

			// Bắt lỗi nhập dữ liệu
			if (!ModelState.IsValid) {
				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? "Error in input data !" : "Có lỗi trong dữ liệu nhập vào !",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);

				return RedirectToAction("Page_Create", model);
			}

			// Lưu bản đồ
			var map = new TblDistributiton {
				Id = Helper.Func.CreateId(),
				ImageMap = "fsfs",
				MapNameEn = model.MapNameEn,
				MapNameVi = model.MapNameVi
			};

			context.TblDistributitons.Add(map);
			await context.SaveChangesAsync();

			// Setup thông báo
			Helper.Notifier.Create(
				Helper.SetupNotifier.Status.success,
				isEN ? "Create successfully." : "Tạo thành công.",
				Helper.SetupNotifier.Timer.fastTime,
				""
			);

			return RedirectToAction("Page_Index");
		}

		// Chỉnh sửa
		public async Task<IActionResult> Page_Edit(string id) {
			try {
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Truy vấn ảnh cho banner slick slider
				var photos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				var photoMangrove = await context.TblPhotos.FirstOrDefaultAsync(item => item.ImageName == mangrove.MainImage);

				// Xử lý thứ tự ảnh banner slick slider
				if (photos.Count() > 1 && photoMangrove != null) {
					photos.Remove(photoMangrove);
					photos.Insert(0, photoMangrove);
				}

				TempData["Photos"] = photos;
				return View(mangrove);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Edit(TblMangrove model, List<IFormFile> ImageFile, List<string> listDesVI, List<string> listDesEN) {
			return View();
		}


		// Chi tiết
		public async Task<IActionResult> Page_Detail(string id) {
			try {
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Truy vấn ảnh cho banner slick slider
				var photos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				var photoMangrove = await context.TblPhotos.FirstOrDefaultAsync(item => item.ImageName == mangrove.MainImage);

				// Xử lý thứ tự ảnh banner slick slider
				if (photos.Count() > 1 && photoMangrove != null) {
					photos.Remove(photoMangrove);
					photos.Insert(0, photoMangrove);
				}

				TempData["Photos"] = photos;
				return View(mangrove);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
	}
}
