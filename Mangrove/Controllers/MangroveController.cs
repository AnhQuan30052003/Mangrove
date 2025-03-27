using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
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
				var info = new InfomationPaginate(listTitle, currentPage, (int)pageSize, fillter.Count(), sortType, sortFollow, findText, "Mangrove", "Page_Index");
				var pagi = new Paginate_VM<TblMangrove>(fillter, info);

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
			var model = new Mangrove_Admin_VM();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(Mangrove_Admin_VM model) {
			bool isEN = Helper.Func.IsEnglish();

			// Begin validate
			Helper.Validate.Clear();
			foreach (var photo in model.Photos) {
				Helper.Validate.NotEmpty(photo.Image.DataBase64);
				Helper.Validate.NotEmpty(photo.NoteImgEn);
				Helper.Validate.NotEmpty(photo.NoteImgVi);
			}
			Helper.Validate.NotEmpty(model.NameEn);
			Helper.Validate.NotEmpty(model.NameVi);
			Helper.Validate.NotEmpty(model.ScientificName);
			Helper.Validate.NotEmpty(model.CommonNameEn);
			Helper.Validate.NotEmpty(model.CommonNameVi);
			Helper.Validate.NotEmpty(model.Familia);
			Helper.Validate.NotEmpty(model.MorphologyEn);
			Helper.Validate.NotEmpty(model.MorphologyVi);
			Helper.Validate.NotEmpty(model.EcologyEn);
			Helper.Validate.NotEmpty(model.EcologyVi);
			Helper.Validate.NotEmpty(model.DistributionEn);
			Helper.Validate.NotEmpty(model.DistributionVi);
			Helper.Validate.NotEmpty(model.ConservationStatusEn);
			Helper.Validate.NotEmpty(model.ConservationStatusVi);
			Helper.Validate.NotEmpty(model.UseEn);
			Helper.Validate.NotEmpty(model.UseVi);

			// Trả lại view nếu có lỗi validate
			if (Helper.Validate.HaveError()) {
				return View(model);
			}
			
			// Khi không có ảnh nào
			if (!model.Photos.Any()) {
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? "Must have at least one photo !" : "Phải có ít nhất một ảnh !",
					Helper.SetupNotifier.Timer.midTime,
					""
				);
				return RedirectToAction("Page_Create");
			}
			// End validate

			// Lưu dữ liệu
			string idMangrve = Helper.Func.CreateId();
			string mainImage = "";

			// Phần ảnh
			for (int i = 0; i < model.Photos.Count(); i++) {
				var photo = model.Photos[i];
				var image = photo.Image;
				string idImage = Helper.Func.CreateId();
				string fileName = $"{idImage}_{model.NameVi}.{image.DataType.Replace("image/", "").Replace("jpeg", "jpg")}";

				bool statusSave = await Helper.Func.SaveImageFromBase64Data(image.DataBase64, Helper.Path.treeImg, fileName);
				if (statusSave) {
					var photoDB = new TblPhoto {
						Id = idImage,
						IdObj = idMangrve,
						ImageName = fileName,
						NoteImgEn = photo.NoteImgEn,
						NoteImgVi = photo.NoteImgVi,
					};
					context.TblPhotos.Add(photoDB);

					if (i == 0) {
						mainImage = fileName;
					}

				}
			}

			// Lưu cây
			var mangrove = new TblMangrove {
				Id = idMangrve,
				NameEn = model.NameEn,
				NameVi = model.NameVi,
				CommonNameEn = model.CommonNameEn,
				CommonNameVi = model.CommonNameVi,
				ScientificName = model.ScientificName,
				Familia = model.Familia,
				MainImage = mainImage,
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
			context.TblMangroves.Add(mangrove);

			await context.SaveChangesAsync();

			// Setup thông báo
			Helper.Notifier.Create(
				Helper.SetupNotifier.Status.success,
				isEN ? "Create successfully." : "Tạo thành công.",
				Helper.SetupNotifier.Timer.fastTime,
				""
			);

			return Content(Helper.Link.GetUrlBack(), "text/html");
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
