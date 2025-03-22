using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq.Expressions;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
			this.context = context;
		}

		// Danh sách cây ngập mặn
		public async Task<IActionResult> Page_Index(string? search = null, int currentPage = 1, int? pageSize = null, string? sortType = null, string? sortFollow = null) {
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.ListPageSize[0];
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				bool isEN = Helper.Func.IsLanguage("EN");

				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Số cá thể", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Number of individuals", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleVI[index++], item => item.NameVi },
					{ listTitleVI[index++], item => item.CommonNameVi },
					{ listTitleVI[index++], item => item.ScientificName },
					{ listTitleVI[index++], item => item.Familia },
					{ listTitleVI[index++], item => item.DistributionVi },
					{ listTitleVI[index++], item => item.TblIndividuals.Count() }
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleEN[index++], item => item.NameEn },
					{ listTitleEN[index++], item => item.CommonNameEn },
					{ listTitleEN[index++], item => item.ScientificName },
					{ listTitleEN[index++], item => item.Familia },
					{ listTitleEN[index++], item => item.DistributionEn },
					{ listTitleEN[index++], item => item.TblIndividuals.Count() }
				};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Tạo query
				var query = context.TblMangroves.Include(item => item.TblIndividuals).AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == "asc" ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();
				//var data = await context.TblMangroves.ToListAsync();

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
				var info = new InfomationPaginate("Both_PaginateTable_IndexMangrove", listTitle, currentPage, (int)pageSize, fillter.Count(), sortType, sortFollow, findText, "Mangrove", "Page_Index");
				var pagi = new PaginateModel<TblMangrove>(fillter, info);

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

		// Tạo cây mới
		public IActionResult Call_Create() {
			var mangrove = new MangroveModel();
			return RedirectToAction("Page_Create", mangrove);
		}
		public IActionResult Page_Create(MangroveModel model) {

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(MangroveModel model, string? info = null) {
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

			if (model.Photos == null) {
				Console.WriteLine($"Photos null");
			}
			else {
				Console.WriteLine($"Photos có {model.Photos.Count()} phân tử");
			}


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

			return RedirectToAction("Page_Index");

			// Lưu cây
			string idMangrve = Helper.Func.CreateId();
			var mangrove = new TblMangrove {
				Id = idMangrve,
				NameEn = model.NameEn,
				NameVi = model.NameVi,
				CommonNameEn = model.CommonNameEn,
				CommonNameVi = model.CommonNameVi,
				ScientificName = model.ScientificName,
				Familia = model.Familia,
				MainImage = "",
				MorphologyEn = model.MorphologyEn,
				MorphologyVi = model.MorphologyVi,
				EcologyEn = model.EcologyEn,
				EcologyVi = model.EcologyVi,
				DistributionEn = model.DistributionEn,
				DistributionVi = model.DistributionVi,
				ConservationStatusEn = model.ConservationStatusEn,
				ConservationStatusVi = model.ConservationStatusVi,
				UseEn = model.UseEn,
				UseVi = model.UseVi,
				View = 0,
				UpdateLast = DateTime.Now
			};

			//List<TblPhoto> listPhotos = new List<TblPhoto>();
			//for (int i = 0; i < ImageFile.Count; i++) {
			//	var file = ImageFile[i];

			//	string fileName = await Helper.Func.SaveImage(Helper.Path.treeImg, file);
			//	if (!string.IsNullOrEmpty(fileName)) {
			//		if (i == 0) mangrove.MainImage = fileName;

			//		var photo = new TblPhoto {
			//			Id = Helper.Func.CreateId(),
			//			IdObj = mangrove.Id,
			//			ImageName = fileName,
			//			NoteImgEn = NoteImgEn[i],
			//			NoteImgVi = NoteImgVi[i]
			//		};
			//		listPhotos.Add(photo);
			//	}
			//}

			context.TblMangroves.Add(mangrove);
			//foreach (var photo in listPhotos) {
			//	context.TblPhotos.Add(photo);
			//}
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

		// Chỉnh sửa cây
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
