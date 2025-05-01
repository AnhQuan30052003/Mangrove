using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	[Authorize]
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
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

				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Số cá thể", "Cập nhật lần cuối", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Number of individuals", "Last updated", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleVI[index++], item => item.NameVi },
					{ listTitleVI[index++], item => item.CommonNameVi },
					{ listTitleVI[index++], item => item.ScientificName },
					{ listTitleVI[index++], item => item.Familia },
					{ listTitleVI[index++], item => item.TblIndividuals.Count() },
					{ listTitleVI[index++], item => item.UpdateLast },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleEN[index++], item => item.NameEn },
					{ listTitleEN[index++], item => item.CommonNameEn },
					{ listTitleEN[index++], item => item.ScientificName },
					{ listTitleEN[index++], item => item.Familia },
					{ listTitleEN[index++], item => item.TblIndividuals.Count() },
					{ listTitleEN[index++], item => item.UpdateLast },
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
					conditions.Add(item.TblIndividuals.Count().ToString());
					conditions.Add(item.UpdateLast.ToShortDateString());

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate(
					listTitle, currentPage, (int)pageSize, fillter.Count(),
					sortType, sortFollow, findText,
					"Mangrove", "Page_Index"
				);
				var pagi = new Paginate_VM<TblMangrove>(fillter, info);

				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
			}
		}

		// Tạo mới
		public IActionResult Page_Create() {
			var model = new TblMangrove();
			Helper.Validate.Clear();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(TblMangrove model, List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Begin validate
				Helper.Validate.Clear();

				for (int i = 0; i < dataTypes.Count(); i++) {
					dataBase64s[i] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[i], dataTypes[i]);
					Helper.Validate.MaxLength(dataBase64s[i], 256);
					Helper.Validate.MaxLength(noteENs[i], 256, true);
					Helper.Validate.MaxLength(noteVIs[i], 256, true);
				}

				Helper.Validate.MaxLength(model.NameEn, 256);
				Helper.Validate.MaxLength(model.NameVi, 256);
				Helper.Validate.MaxLength(model.ScientificName, 256);
				Helper.Validate.MaxLength(model.CommonNameEn, 256, true);
				Helper.Validate.MaxLength(model.CommonNameVi, 256, true);
				Helper.Validate.MaxLength(model.Familia, 256);
				Helper.Validate.NotEmpty(model.MorphologyEn);
				Helper.Validate.NotEmpty(model.MorphologyVi);
				Helper.Validate.NotEmpty(model.EcologyEn);
				Helper.Validate.NotEmpty(model.EcologyVi);
				Helper.Validate.NotEmpty(model.UseEn);
				Helper.Validate.NotEmpty(model.UseVi);
				Helper.Validate.MaxLength(model.TitleDistributionEn, 256);
				Helper.Validate.MaxLength(model.TitleDistributionVi, 256);
				Helper.Validate.MaxLength(model.DistributionEn, 256);
				Helper.Validate.MaxLength(model.DistributionVi, 256);
				Helper.Validate.MaxLength(model.ConservationStatusEn, 256);
				Helper.Validate.MaxLength(model.ConservationStatusVi, 256);

				TempData["DataTypes"] = dataTypes;
				TempData["DataBase64s"] = dataBase64s;
				TempData["NoteENs"] = noteENs;
				TempData["NoteVIs"] = noteVIs;

				// Khi không có ảnh nào
				if (!dataBase64s.Any()) {
					Helper.Notifier.Fail(
						isEN ? "Must have at least one photo !" : "Phải có ít nhất một ảnh !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Lưu dữ liệu
				// Lưu cây
				model.Id = Helper.Func.CreateId();
				model.View = 0;
				model.UpdateLast = DateTime.Now;
				context.TblMangroves.Add(model);

				// Phần ảnh
				for (int i = 0; i < dataBase64s.Count(); i++) {
					string idPhoto = Helper.Func.CreateId();
					string fileName = $"{idPhoto}_{model.NameVi}{Helper.Func.GetTypeImage(dataTypes[i])}";

					// Chuyển ảnh vào đúng thư mục
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[i]),
						Path.Combine(Helper.Path.treeImg, fileName)
					);

					if (i == 0) {
						model.MainImage = fileName;
					}

					var photoDB = new TblPhoto {
						Id = idPhoto,
						IdObj = model.Id,
						ImageName = fileName,
						NoteImgEn = noteENs[i],
						NoteImgVi = noteVIs[i],
						NumberOrder = i
					};
					context.TblPhotos.Add(photoDB);
				}

				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo
				Helper.Notifier.Success(
					isEN ? "Create successfully." : "Tạo thành công.",
					Helper.SetupNotifier.Timer.fastTime
				);
				return RedirectToAction("Page_Detail", new { id = model.Id });
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "There was an error adding the mangrove. Please try again later !" : "Có lỗi trong quá trình thêm cây ngập mặn. Vui lòng thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View(model);
			}
		}

		// Chỉnh sửa
		public async Task<IActionResult> Page_Edit(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var model = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == id);
				if (model == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				// Danh sách hình ảnh
				var listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == model.Id)
				.OrderBy(item => item.NumberOrder)
				.ToListAsync();

				// Xử lý thứ tự ảnh banner slick slider
				if (listPhoto.Count() > 1) {
					foreach (var photo in listPhoto) {
						if (photo.ImageName == model.MainImage) {
							var save = photo;
							listPhoto.Remove(save);
							listPhoto.Insert(0, save);
							break;
						}
					}
				}

				var dataTypes = new List<string>();
				var dataBase64s = new List<string>();
				var noteENs = new List<string>();
				var noteVIs = new List<string>();
				foreach (var photo in listPhoto) {
					dataTypes.Add(string.Empty);
					dataBase64s.Add(photo.ImageName);
					noteENs.Add(photo.NoteImgEn ?? string.Empty);
					noteVIs.Add(photo.NoteImgVi ?? string.Empty);
				}

				TempData["DataTypes"] = dataTypes;
				TempData["DataBase64s"] = dataBase64s;
				TempData["NoteENs"] = noteENs;
				TempData["NoteVIs"] = noteVIs;

				Helper.Validate.Clear();
				return View(model);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access edit status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageListIndex), "text/html");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Edit(TblMangrove model, List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				TempData["DataTypes"] = dataTypes;
				TempData["DataBase64s"] = dataBase64s;
				TempData["NoteENs"] = noteENs;
				TempData["NoteVIs"] = noteVIs;

				// Begin validate
				Helper.Validate.Clear();

				for (int i = 0; i < dataTypes.Count(); i++) {
					dataBase64s[i] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[i], dataTypes[i]);
					Helper.Validate.MaxLength(dataBase64s[i], 256);
					Helper.Validate.MaxLength(noteENs[i], 256, true);
					Helper.Validate.MaxLength(noteVIs[i], 256, true);
				}

				Helper.Validate.MaxLength(model.NameEn, 256);
				Helper.Validate.MaxLength(model.NameVi, 256);
				Helper.Validate.MaxLength(model.ScientificName, 256);
				Helper.Validate.MaxLength(model.CommonNameEn, 256, true);
				Helper.Validate.MaxLength(model.CommonNameVi, 256, true);
				Helper.Validate.MaxLength(model.Familia, 256);
				Helper.Validate.NotEmpty(model.MorphologyEn);
				Helper.Validate.NotEmpty(model.MorphologyVi);
				Helper.Validate.NotEmpty(model.EcologyEn);
				Helper.Validate.NotEmpty(model.EcologyVi);
				Helper.Validate.NotEmpty(model.UseEn);
				Helper.Validate.NotEmpty(model.UseVi);
				Helper.Validate.MaxLength(model.TitleDistributionEn, 256);
				Helper.Validate.MaxLength(model.TitleDistributionVi, 256);
				Helper.Validate.MaxLength(model.DistributionEn, 256);
				Helper.Validate.MaxLength(model.DistributionVi, 256);
				Helper.Validate.MaxLength(model.ConservationStatusEn, 256);
				Helper.Validate.MaxLength(model.ConservationStatusVi, 256);

				// Khi không có ảnh nào
				if (!dataBase64s.Any()) {
					Helper.Notifier.Fail(
						isEN ? "Must have at least one photo !" : "Phải có ít nhất một ảnh !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Lưu dữ liệu
				// Lưu cây
				model.CommonNameEn = model.CommonNameEn ?? string.Empty;
				model.CommonNameVi = model.CommonNameVi ?? string.Empty;
				model.UpdateLast = DateTime.Now;
				context.TblMangroves.Update(model);

				// Phẩn ảnh - Lưu ảnh mới về (mới này có thể có luôn ảnh cũ)
				List<string> saveIdPhoto = new List<string>();
				for (int i = 0; i < dataBase64s.Count(); i++) {
					// Setup cho ảnh cũ
					string idPhoto = Helper.Func.GetIdFromFileName(dataBase64s[i]);
					string fileName = $"{idPhoto}_{model.NameVi}";
					string oldPath = Path.Combine(Helper.Path.treeImg, dataBase64s[i]);

					// Nếu là ảnh mới
					if (dataBase64s[i].Contains(Helper.Key.temp)) {
						fileName += Helper.Func.GetTypeImage(dataTypes[i]);
						oldPath = Path.Combine(Helper.Path.temptImg, dataBase64s[i]);

						var newPhoto = new TblPhoto {
							Id = idPhoto,
							IdObj = model.Id,
							ImageName = fileName,
							NoteImgEn = noteENs[i],
							NoteImgVi = noteVIs[i],
							NumberOrder = i
						};
						context.TblPhotos.Add(newPhoto);
					}
					else {
						fileName += Path.GetExtension(dataBase64s[i]);

						var photo = await context.TblPhotos.FirstOrDefaultAsync(item => item.Id == idPhoto);
						if (photo != null) {
							photo.ImageName = fileName;
							photo.NoteImgEn = noteENs[i];
							photo.NoteImgVi = noteVIs[i];
							photo.NumberOrder = i;
							context.TblPhotos.Update(photo);

							idPhoto = photo.Id;
						}
					}

					// Lưu lại tên ảnh này
					if (i == 0) {
						model.MainImage = fileName;
					}
					saveIdPhoto.Add(idPhoto);

					// Chuyển ảnh vào đúng thư mục
					string newPath = Path.Combine(Helper.Path.treeImg, fileName);
					Helper.Func.MovePhoto(oldPath, newPath);
				}

				// Phần ảnh - Xử lý, xoá đi các ảnh cũ!
				var photoMangrove = await context.TblPhotos.Where(item => item.IdObj == model.Id).ToListAsync();
				if (photoMangrove.Any()) {
					foreach (var photo in photoMangrove) {
						if (!saveIdPhoto.Contains(photo.Id)) {
							Helper.Func.DeletePhoto(Helper.Path.treeImg, photo.ImageName);
							context.TblPhotos.Remove(photo);
						}
					}
				}
				await context.SaveChangesAsync();

				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);
				return RedirectToAction("Page_Detail", new { id = model.Id });
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Edit request failed. Please try again later !" : "Yêu cầu chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View(model);
			}
		}

		// Chi tiết
		public async Task<IActionResult> Page_Detail(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var model = await context.TblMangroves
				.Select(item => new Mangrove_Client_VM {
					Id = item.Id,
					MainImage = item.MainImage,
					Name = isEN ? item.NameEn : item.NameVi,
					CommonName = isEN ? item.CommonNameEn : item.CommonNameVi,
					ScientificName = item.ScientificName,
					Familia = item.Familia,
					Use = isEN ? item.UseEn : item.UseVi,
					Morphology = isEN ? item.MorphologyEn : item.MorphologyVi,
					Ecology = isEN ? item.EcologyEn : item.EcologyVi,
					TitleDistribution = (isEN ? item.TitleDistributionEn : item.TitleDistributionVi) ?? string.Empty,
					Distribution = isEN ? item.DistributionEn : item.DistributionVi,
					ConservationStatus = isEN ? item.ConservationStatusEn : item.ConservationStatusVi,
					View = item.View,
				})
				.FirstOrDefaultAsync(item => item.Id == id);

				if (model == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				// Danh sách hình ảnh
				List<Photo_Mangrove_Client_VM> listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == id)
				.OrderBy(item => item.NumberOrder)
				.Select(item => new Photo_Mangrove_Client_VM {
					Image = item.ImageName,
					Note = (isEN ? item.NoteImgEn : item.NoteImgVi) ?? ""
				})
				.ToListAsync();

				// Xử lý thứ tự ảnh banner slick slider
				if (listPhoto.Count() > 1) {
					foreach (var photo in listPhoto) {
						if (photo.Image == model.MainImage) {
							var save = photo;
							listPhoto.Remove(save);
							listPhoto.Insert(0, save);
							break;
						}
					}
				}

				model.Photos = listPhoto;

				return View(model);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access detail status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chi tiết thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageListIndex), "text/html");
			}
		}

		// Xoá
		public async Task<IActionResult> Page_Delete(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Kiểm tra có cá thể nào không ?
				var individuals = await context.TblIndividuals.Where(item => item.IdMangrove == id).ToListAsync();
				if (individuals.Any()) {
					Helper.Notifier.Fail(
						isEN ? "Can only delete mangrove trees without any individuals !" : "Chỉ có thể xoá cây ngập mặn không có cá thể nào !",
						Helper.SetupNotifier.Timer.midTime
					);
					return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageDelete), "text/html");
				}

				// Xoá đối tượng
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}
				context.TblMangroves.Remove(mangrove);
				await context.SaveChangesAsync();

				// Xoá ảnh
				var mangrovePhotos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				if (mangrovePhotos.Any()) {
					foreach (var photo in mangrovePhotos) {
						context.TblPhotos.Remove(photo);
						Helper.Func.DeletePhoto(Helper.Path.treeImg, photo.ImageName);
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

		// Thông tin chung
		public async Task<IActionResult> Page_Overview_View() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var info = await context.TblInforOverviews.FirstOrDefaultAsync();
				// Danh sách hình ảnh
				List<TblPhoto> listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == info.Id)
				.OrderBy(item => item.NumberOrder)
				.ToListAsync();

				ViewData["Photos"] = listPhoto;

				return View(info);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The Overview of mangrove plants page is currently inaccessible !" : "Trang tổng quan thực vật ngập mặn hiện tại không thể truy cập !",
					Helper.SetupNotifier.Timer.shortTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageOverviewMangrove), "text/html");
			}
		}

		// Thông tin chung
		public async Task<IActionResult> Page_Overview_Edit() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var info = await context.TblInforOverviews.FirstOrDefaultAsync();
				
				// Danh sách hình ảnh
				var listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == info.Id)
				.OrderBy(item => item.NumberOrder)
				.ToListAsync();

				var dataTypes = new List<string>();
				var dataBase64s = new List<string>();
				var noteENs = new List<string>();
				var noteVIs = new List<string>();
				foreach (var photo in listPhoto) {
					dataTypes.Add(string.Empty);
					dataBase64s.Add(photo.ImageName);
					noteENs.Add(photo.NoteImgEn ?? string.Empty);
					noteVIs.Add(photo.NoteImgVi ?? string.Empty);
				}

				ViewData["DataTypes"] = dataTypes;
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				Helper.Validate.Clear();
				return View(info);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access edit status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageOverviewMangrove), "text/html");
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Overview_Edit(TblInforOverview model, List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				ViewData["DataTypes"] = dataTypes;
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				// Begin validate
				Helper.Validate.Clear();
				for (int i = 0; i < dataTypes.Count(); i++) {
					dataBase64s[i] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[i], dataTypes[i]);
					Helper.Validate.MaxLength(dataBase64s[i], 256);
					Helper.Validate.MaxLength(noteENs[i], 256, true);
					Helper.Validate.MaxLength(noteVIs[i], 256, true);
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Save Data
				// Phẩn ảnh - Lưu ảnh mới về (mới này có thể có luôn ảnh cũ)
				List<string> saveIdPhoto = new List<string>();
				for (int i = 0; i < dataBase64s.Count(); i++) {
					// Setup cho ảnh cũ
					string idPhoto = Helper.Func.GetIdFromFileName(dataBase64s[i]);
					string fileName = $"{idPhoto}_{noteVIs[i] ?? string.Empty}";
					string oldPath = Path.Combine(Helper.Path.treeImg, dataBase64s[i]);

					// Nếu là ảnh mới
					if (dataBase64s[i].Contains(Helper.Key.temp)) {
						fileName += Helper.Func.GetTypeImage(dataTypes[i]);
						oldPath = Path.Combine(Helper.Path.temptImg, dataBase64s[i]);

						var newPhoto = new TblPhoto {
							Id = idPhoto,
							IdObj = model.Id,
							ImageName = fileName,
							NoteImgEn = noteENs[i],
							NoteImgVi = noteVIs[i],
							NumberOrder = i
						};
						context.TblPhotos.Add(newPhoto);
					}
					else {
						fileName += Path.GetExtension(dataBase64s[i]);

						var photo = await context.TblPhotos.FirstOrDefaultAsync(item => item.Id == idPhoto);
						if (photo != null) {
							photo.ImageName = fileName;
							photo.NoteImgEn = noteENs[i];
							photo.NoteImgVi = noteVIs[i];
							photo.NumberOrder = i;
							context.TblPhotos.Update(photo);

							idPhoto = photo.Id;
						}
					}
					saveIdPhoto.Add(idPhoto);

					// Chuyển ảnh vào đúng thư mục
					string newPath = Path.Combine(Helper.Path.overviewImg, fileName);
					Helper.Func.MovePhoto(oldPath, newPath);
				}

				// Phần ảnh - Xử lý, xoá đi các ảnh cũ!
				var photoSaved = await context.TblPhotos.Where(item => item.IdObj == model.Id).ToListAsync();
				if (photoSaved.Any()) {
					foreach (var photo in photoSaved) {
						if (!saveIdPhoto.Contains(photo.Id)) {
							Helper.Func.DeletePhoto(Helper.Path.overviewImg, photo.ImageName);
							context.TblPhotos.Remove(photo);
						}
					}
				}
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);
			
				// Cập nhật model
				model.InforEn = model.InforEn ?? string.Empty;
				model.InforVi = model.InforVi ?? string.Empty;
				context.TblInforOverviews.Update(model);
				await context.SaveChangesAsync();

				// Setup thông báo thành công
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);
				return RedirectToAction("Page_Overview_View");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Edit request failed. Please try again later !" : "Yêu cầu chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageOverviewMangrove), "text/html");
			}
		}
	}
}
