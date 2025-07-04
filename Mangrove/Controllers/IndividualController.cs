﻿using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mangrove.Controllers {
	[Authorize]
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

				var listTitleVI = new List<string> { "STT", "Tên cá thể", "Vị trí", "Kinh độ", "Vĩ độ", "Số giai đoạn", "Cập nhật lần cuối", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Individual name", "Position", "Longitude", "Latitude", "Number of stage", "Last updated", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
					{ listTitleVI[index++], item => item.IdMangroveNavigation!.NameVi },
					{ listTitleVI[index++], item => item.PositionVi },
					{ listTitleVI[index++], item => item.Longitude ?? string.Empty },
					{ listTitleVI[index++], item => item.Latitude ?? string.Empty },
					{ listTitleVI[index++], item => item.TblStages.Count() },
					{ listTitleVI[index++], item => item.UpdateLast },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblIndividual, object>>>()
				{
					{ listTitleEN[index++], item => item.IdMangroveNavigation!.NameEn },
					{ listTitleEN[index++], item => item.PositionEn },
					{ listTitleEN[index++], item => item.Longitude ?? string.Empty },
					{ listTitleEN[index++], item => item.Latitude ?? string.Empty },
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
		public async Task<IActionResult> Page_Create() {
			// Tạo model
			var model = new TblIndividual();

			// Tạo select
			var mangroves = await context.TblMangroves
			.OrderBy(item => Helper.Func.IsEnglish() ? item.NameEn : item.NameVi)
			.ToListAsync();
			ViewData["Mangroves"] = mangroves;

			Helper.Validate.Clear();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(TblIndividual model, string chooseIdMangrove,
			List<int> indexStages, List<string> activeStages, List<string> itemPhotoOfStages, List<string> idStages,
			List<DateTime> surveyDates, List<string> stageNameENs, List<string> stageNameVIs, List<string> weatherENs, List<string> weatherVIs, List<string> heights, List<string> perimeters, 
			List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Tạm lưu lại dữ liệu cho view
				ViewData["ChooseIdMangrove"] = chooseIdMangrove;
				ViewData["IndexStages"] = indexStages;
				ViewData["ActiveStages"] = activeStages;
				ViewData["ItemPhotoOfStages"] = itemPhotoOfStages;
				ViewData["IdStages"] = idStages;

				ViewData["SurveyDates"] = surveyDates;
				ViewData["StageNameENs"] = stageNameENs;
				ViewData["StageNameVIs"] = stageNameVIs;
				ViewData["WeatherENs"] = weatherENs;
				ViewData["WeatherVIs"] = weatherVIs;

				ViewData["DataTypes"] = dataTypes;
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;
				ViewData["Heights"] = heights;
				ViewData["Perimeters"] = perimeters;

				// Tạo select
				var mangroves = await context.TblMangroves.ToListAsync();
				ViewData["Mangroves"] = mangroves;

				// Begin validate
				Helper.Validate.Clear();
				Helper.Validate.MaxLength(model.Longitude, 256, true);
				Helper.Validate.MaxLength(model.Latitude, 256, true);
				Helper.Validate.MaxLength(model.PositionEn, 256);
				Helper.Validate.MaxLength(model.PositionVi, 256);

				int indexPhoto = 0, indexNote = 0;
				for (int i = 0; i < indexStages.Count(); i++) {
					dataBase64s[indexPhoto] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[indexPhoto], dataTypes[indexPhoto]);
					Helper.Validate.MaxLength(dataBase64s[indexPhoto], 256);

					Helper.Validate.NotEmpty(surveyDates[i].ToString("yyyy-MM-dd"));
					Helper.Validate.MaxLength(stageNameENs[i], 256, true);
					Helper.Validate.MaxLength(stageNameVIs[i], 256, true);
					Helper.Validate.MaxLength(weatherENs[i], 256, true);
					Helper.Validate.MaxLength(weatherVIs[i], 256, true);
					Helper.Validate.MaxLength(heights[i], 100, true);
					Helper.Validate.MaxLength(perimeters[i], 100, true);

					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						dataBase64s[j] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[j], dataTypes[j]);
						Helper.Validate.MaxLength(dataBase64s[j], 256);
						Helper.Validate.MaxLength(noteENs[indexNote], 256, true);
						Helper.Validate.MaxLength(noteVIs[indexNote], 256, true);
						indexNote += 1;
					}
					indexPhoto += countItemPhotoOfStage + 1;
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

				// Save data
				// Truy vấn tên thành phần loài 
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == chooseIdMangrove);
				string nameMangrove = mangrove != null ? mangrove.NameVi : string.Empty;

				// Lưu cá thể
				model.Id = Helper.Func.CreateId();
				model.IdMangrove = chooseIdMangrove;
				model.View = 0;
				model.QrName = $"QR_{model.Id}_{nameMangrove}_{model.PositionVi}.png";
				model.UpdateLast = DateTime.Now;
				context.TblIndividuals.Add(model);
				Helper.Func.CreateQRCode($"{Helper.Link.hosting}/Home/Page_Individual/{model.Id}", model.QrName);

				indexPhoto = indexNote = 0;
				for (int i = 0; i < indexStages.Count(); i++) {
					// Lưu giai đoạn
					string idStage = Helper.Func.CreateId();
					string fileName = $"{idStage}_{nameMangrove}{Helper.Func.GetTypeImage(dataTypes[indexPhoto])}";
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[indexPhoto]),
						Path.Combine(Helper.Path.stageImg, fileName)
					);
					var newStage = new TblStage {
						Id = idStage,
						IdIndividual = model.Id,
						MainImage = fileName,
						SurveyDay = surveyDates[i],
						NameEn = stageNameENs[i] ?? $"Stages {i + 1}",
						NameVi = stageNameVIs[i] ?? $"Giai đoạn {i + 1}",
						WeatherEn = weatherENs[i],
						WeatherVi = weatherVIs[i],
						Height = heights[i],
						Perimeter = perimeters[i],
						NumberOrder = i
					};
					context.TblStages.Add(newStage);

					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						// Lưu ảnh
						string idPhoto = Helper.Func.CreateId();
						fileName = $"{idPhoto}_{noteVIs[indexNote]}{Helper.Func.GetTypeImage(dataTypes[indexNote])}";
						Helper.Func.MovePhoto(
							Path.Combine(Helper.Path.temptImg, dataBase64s[j]),
							Path.Combine(Helper.Path.stageImg, fileName)
						);
						var newPhoto = new TblPhoto {
							Id = idPhoto,
							IdObj = idStage,
							ImageName = fileName,
							NoteImgEn = noteENs[indexNote],
							NoteImgVi = noteVIs[indexNote],
							NumberOrder = indexNote
						};
						context.TblPhotos.Add(newPhoto);

						indexNote += 1;
					}
					indexPhoto += countItemPhotoOfStage + 1;
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
			catch (Exception ex) {
				Helper.Notifier.Fail(
					isEN ? "There was an error adding the individual. Please try again later !" : "Có lỗi trong quá trình thêm cá thể. Vui lòng thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				Console.WriteLine($"Error:\n{ex.Message}");
				return View(model);
			}
		}

		// Edit
		public async Task<IActionResult> Page_Edit(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Truy vấn cá thể
				var individual = await context.TblIndividuals
				.Include(item => item.TblStages)
				.FirstOrDefaultAsync(item => item.Id == id);
				if (individual == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				string idMangrove = individual.IdMangrove ?? string.Empty;
				ViewData["ChooseIdMangrove"] = idMangrove;
				ViewData["Mangroves"] = await context.TblMangroves.OrderBy(item => isEN ? item.NameEn : item.NameVi).ToListAsync();

				List<int> indexStages = new List<int>();
				List<string> activeStages = new List<string>();
				List<string> itemPhotoOfStages = new List<string>();
				List<string> idStages = new List<string>();

				List<DateTime> surveyDats = new List<DateTime>();
				List<string> stageNameENs = new List<string>();
				List<string> stageNameVIs = new List<string>();
				List<string> weatherENs = new List<string>();
				List<string> weatherVIs = new List<string>();
				List<string> heights = new List<string>();
				List<string> perimeters = new List<string>();

				List<string> dataBase64s = new List<string>();
				List<string> dataTypes = new List<string>();
				List<string> noteENs = new List<string>();
				List<string> noteVIs = new List<string>();

				int i = 0;
				foreach (var stage in individual.TblStages.OrderBy(item => item.NumberOrder).ToList()) {
					indexStages.Add(i + 1);
					activeStages.Add(i == 0 ? "active" : string.Empty);

					//var stage = individual.TblStages.ElementAt(i);

					var listPhotos = await context.TblPhotos
					.Where(item => item.IdObj == stage.Id)
					.OrderBy(item => item.NumberOrder)
					.ToListAsync();
					itemPhotoOfStages.Add(listPhotos.Count().ToString());
					idStages.Add(stage.Id);

					surveyDats.Add(stage.SurveyDay);

					stageNameENs.Add(stage.NameEn);
					stageNameVIs.Add(stage.NameVi);
					weatherENs.Add(stage.WeatherEn ?? string.Empty);
					weatherVIs.Add(stage.WeatherVi ?? string.Empty);
					heights.Add(stage.Height ?? string.Empty);
					perimeters.Add(stage.Perimeter ?? string.Empty);

					dataBase64s.Add(stage.MainImage);
					dataTypes.Add(string.Empty);
					foreach (var photo in listPhotos) {
						dataBase64s.Add(photo.ImageName);
						dataTypes.Add(string.Empty);
						noteENs.Add(photo.NoteImgEn ?? string.Empty);
						noteVIs.Add(photo.NoteImgVi ?? string.Empty);
					}
					i += 1;
				}

				ViewData["IndexStages"] = indexStages;
				ViewData["ActiveStages"] = activeStages;
				ViewData["ItemPhotoOfStages"] = itemPhotoOfStages;
				ViewData["IdStages"] = idStages;

				ViewData["SurveyDates"] = surveyDats;
				ViewData["StageNameENs"] = stageNameENs;
				ViewData["StageNameVIs"] = stageNameVIs;
				ViewData["WeatherENs"] = weatherENs;
				ViewData["WeatherVIs"] = weatherVIs;
				ViewData["Heights"] = heights;
				ViewData["Perimeters"] = perimeters;

				ViewData["DataBase64s"] = dataBase64s;
				ViewData["DataTypes"] = dataTypes;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				Helper.Validate.Clear();
				return View(individual);
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
		public async Task<IActionResult> Page_Edit(TblIndividual model, string chooseIdMangrove,
			List<int> indexStages, List<string> activeStages, List<string> itemPhotoOfStages, List<string> idStages,
			List<DateTime> surveyDates, List<string> stageNameENs, List<string> stageNameVIs, List<string> weatherENs, List<string> weatherVIs, List<string> heights, List<string> perimeters, 
			List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {

			bool isEN = Helper.Func.IsEnglish();
			try {
				// Tạm lưu lại dữ liệu cho view
				ViewData["ChooseIdMangrove"] = chooseIdMangrove;
				ViewData["IndexStages"] = indexStages;
				ViewData["ActiveStages"] = activeStages;
				ViewData["ItemPhotoOfStages"] = itemPhotoOfStages;
				ViewData["IdStages"] = idStages;

				ViewData["SurveyDates"] = surveyDates;
				ViewData["StageNameENs"] = stageNameENs;
				ViewData["StageNameVIs"] = stageNameVIs;
				ViewData["WeatherENs"] = weatherENs;
				ViewData["WeatherVIs"] = weatherVIs;
				ViewData["Heights"] = heights;
				ViewData["Perimeters"] = perimeters;

				ViewData["DataTypes"] = dataTypes;
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				// Tạo select
				var mangroves = await context.TblMangroves.ToListAsync();
				ViewData["Mangroves"] = mangroves;

				// Begin validate
				Helper.Validate.Clear();
				Helper.Validate.MaxLength(model.Longitude, 256, true);
				Helper.Validate.MaxLength(model.Latitude, 256, true);
				Helper.Validate.MaxLength(model.PositionEn, 256);
				Helper.Validate.MaxLength(model.PositionVi, 256);

				int indexPhoto = 0, indexNote = 0;
				for (int i = 0; i < indexStages.Count(); i++) {
					dataBase64s[indexPhoto] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[indexPhoto], dataTypes[indexPhoto]);
					Helper.Validate.MaxLength(dataBase64s[indexPhoto], 256);

					Helper.Validate.NotEmpty(surveyDates[i].ToString("yyyy-MM-dd"));
					Helper.Validate.MaxLength(stageNameENs[i], 256, true);
					Helper.Validate.MaxLength(stageNameVIs[i], 256, true);
					Helper.Validate.MaxLength(weatherENs[i], 256, true);
					Helper.Validate.MaxLength(weatherVIs[i], 256, true);
					Helper.Validate.MaxLength(heights[i], 100, true);
					Helper.Validate.MaxLength(perimeters[i], 100, true);

					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						dataBase64s[j] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[j], dataTypes[j]);
						Helper.Validate.MaxLength(dataBase64s[j], 256);
						Helper.Validate.MaxLength(noteENs[indexNote], 256, true);
						Helper.Validate.MaxLength(noteVIs[indexNote], 256, true);
						indexNote += 1;
					}
					indexPhoto += countItemPhotoOfStage + 1;
				}

				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End validate

				// Save data
				// Truy vấn tên thành phần loài		
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == chooseIdMangrove);
				string nameMangrove = mangrove != null ? mangrove.NameVi : string.Empty;

				// xử lý QR cũ
				Helper.Func.DeletePhoto(Helper.Path.qrImg, model.QrName);
				model.QrName = $"QR_{model.Id}_{nameMangrove}_{model.PositionVi}.png";
				Helper.Func.CreateQRCode($"{Helper.Link.hosting}/Home/Page_Individual/{model.Id}", model.QrName);

				// Cập nhật model
				model.IdMangrove = chooseIdMangrove;
				model.UpdateLast = DateTime.Now;
				context.TblIndividuals.Update(model);

				List<string> saveIdStage = new List<string>();
				List<string> saveIdPhoto = new List<string>();

				indexPhoto = indexNote = 0;
				string fileName = string.Empty;
				for (int i = 0; i < indexStages.Count(); i++) {
					// Cập nhật giai đoạn
					string idStage = Helper.Func.CreateId();
					if (string.IsNullOrEmpty(idStages[i])) {
						fileName = $"{idStage}_{nameMangrove}{Helper.Func.GetTypeImage(dataTypes[indexPhoto])}";
						// Tạo mới giai đoạn & thêm vào DB
						var newStage = new TblStage {
							Id = idStage,
							IdIndividual = model.Id,
							MainImage = fileName,
							SurveyDay = surveyDates[i],
							NameEn = stageNameENs[i] ?? $"Stages {i + 1}",
							NameVi = stageNameVIs[i] ?? $"Giai đoạn {i + 1}",
							WeatherEn = weatherENs[i],
							WeatherVi = weatherVIs[i],
							Height = heights[i],
							Perimeter = perimeters[i],
							NumberOrder = i
						};
						context.TblStages.Add(newStage);

						// Lưu ảnh
						Helper.Func.MovePhoto(
							Path.Combine(Helper.Path.temptImg, dataBase64s[indexPhoto]),
							Path.Combine(Helper.Path.stageImg, fileName)
						);
					}
					else {
						// Tìm giai đoạn ấy
						var oldStage = await context.TblStages.FirstOrDefaultAsync(item => item.Id == idStages[i]);
						if (oldStage != null) {
							oldStage.SurveyDay = surveyDates[i];
							oldStage.NameEn = stageNameENs[i] ?? $"Stages {i + 1}";
							oldStage.NameVi = stageNameVIs[i] ?? $"Giai đoạn {i + 1}";
							oldStage.WeatherEn = weatherENs[i];
							oldStage.WeatherVi = weatherVIs[i];
							oldStage.Height = heights[i];
							oldStage.Perimeter = perimeters[i];
							oldStage.NumberOrder = i;

							// Lưu ảnh
							// setup lấy từ nguồn 
							fileName = $"{oldStage.Id}_{nameMangrove}";
							string oldPath = Path.Combine(Helper.Path.stageImg, dataBase64s[indexPhoto]);
							// Ảnh đại diện mới
							if (dataBase64s[indexPhoto].Contains(Helper.Key.temp)) {
								fileName += Helper.Func.GetTypeImage(dataTypes[indexPhoto]);
								oldPath = Path.Combine(Helper.Path.temptImg, dataBase64s[indexPhoto]);
								Helper.Func.DeletePhoto(Helper.Path.stageImg, oldStage.MainImage);
							}
							else {
								fileName += Path.GetExtension(oldStage.MainImage);
							}

							string newPath = Path.Combine(Helper.Path.stageImg, fileName);
							Helper.Func.MovePhoto(oldPath, newPath);

							oldStage.MainImage = fileName;
							context.TblStages.Update(oldStage);
							// Gáng lại id cho giai đoạn đang thao tác để thêm vào SaveIdStage
							idStage = oldStage.Id;
						}
					}
					saveIdStage.Add(idStage);

					// Cập nhật ảnh của giai đoạn
					saveIdPhoto.Clear();
					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						// Lưu ảnh
						// Setup cho ảnh cũ
						string idPhoto = Helper.Func.GetIdFromFileName(dataBase64s[j]);
						fileName = $"{idPhoto}_{noteVIs[indexNote]}";
						string oldPath = Path.Combine(Helper.Path.stageImg, dataBase64s[j]);

						// Nếu là ảnh mới
						if (dataBase64s[j].Contains(Helper.Key.temp)) {
							fileName += Helper.Func.GetTypeImage(dataTypes[j]);
							oldPath = Path.Combine(Helper.Path.temptImg, dataBase64s[j]);

							var newPhoto = new TblPhoto {
								Id = idPhoto,
								IdObj = idStage,
								ImageName = fileName,
								NoteImgEn = noteENs[indexNote],
								NoteImgVi = noteVIs[indexNote],
								NumberOrder = indexNote
							};
							context.TblPhotos.Add(newPhoto);
						}
						else {
							fileName += Path.GetExtension(dataBase64s[j]);

							var photo = await context.TblPhotos.FirstOrDefaultAsync(item => item.Id == idPhoto);
							if (photo != null) {
								photo.ImageName = fileName;
								photo.NoteImgEn = noteENs[indexNote];
								photo.NoteImgVi = noteVIs[indexNote];
								photo.NumberOrder = indexNote;
								context.TblPhotos.Update(photo);
							}
						}
						saveIdPhoto.Add(idPhoto);
						string newPath = Path.Combine(Helper.Path.stageImg, fileName);
						Helper.Func.MovePhoto(oldPath, newPath);
						indexNote += 1;
					}
					indexPhoto += countItemPhotoOfStage + 1;

					// Xoá ảnh cũ của giai đoạn này
					var photoStage = await context.TblPhotos.Where(item => item.IdObj == idStage).ToListAsync();
					if (photoStage.Any()) {
						foreach (var photo in photoStage) {
							if (!saveIdPhoto.Contains(photo.Id)) {
								Helper.Func.DeletePhoto(Helper.Path.stageImg, photo.ImageName);
								context.TblPhotos.Remove(photo);
							}
						}
					}
					await context.SaveChangesAsync();
				}

				// Xoá giai đoạn cũ không dùng của cá thể
				var stageOfInvidual = await context.TblStages.Where(item => item.IdIndividual == model.Id).ToListAsync();
				if (stageOfInvidual.Any()) {
					foreach (var stage in stageOfInvidual) {
						if (!saveIdStage.Contains(stage.Id)) {
							// Xoá ảnh của giai đoạn
							var photos = await context.TblPhotos.Where(item => item.IdObj == stage.Id).ToListAsync();
							if (photos.Any()) {
								foreach (var photo in photos) {
									Helper.Func.DeletePhoto(Helper.Path.stageImg, photo.ImageName);
									context.TblPhotos.Remove(photo);
								}
							}

							// Xoả ảnh đại diện giai đoạn này
							Helper.Func.DeletePhoto(Helper.Path.stageImg, stage.MainImage);

							// Xoá giai đoạn này
							context.TblStages.Remove(stage);
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
				foreach (var stage in individual.TblStages.OrderBy(item => item.NumberOrder).ToList()) {
					List<TblPhoto> photos = await context.TblPhotos
					.Where(item => item.IdObj == stage.Id)
					.OrderBy(item => item.NumberOrder)
					.ToListAsync();
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
				// Kiểm tra có giai đoạn nào không ?
				var stages = await context.TblStages.Where(item => item.IdIndividual == id).ToListAsync();
				if (stages.Any()) {
					Helper.Notifier.Fail(
						isEN ? "Can only delete individuals without any stages !" : "Chỉ có thể xoá cá thể không có giai đoạn nào !",
						Helper.SetupNotifier.Timer.midTime
					);
					return Content(Helper.Link.ScriptGetUrlBack(Helper.Key.adminToPageDelete), "text/html");
				}

				// Xoá đối tượng
				var individual = await context.TblIndividuals.FirstOrDefaultAsync(item => item.Id == id);
				if (individual == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}
				context.TblIndividuals.Remove(individual);

				// Xoá QR
				Helper.Func.DeletePhoto(Helper.Path.qrImg, individual.QrName);

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
