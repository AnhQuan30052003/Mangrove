using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
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
		public async Task<IActionResult> Page_Create() {
			// Tạo model
			var model = new TblIndividual();

			// Tạo select
			var mangroves = await context.TblMangroves.ToListAsync();
			ViewData["Mangroves"] = mangroves;

			Helper.Validate.Clear();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(TblIndividual model, string chooseIdMangrove, List<int> indexStages, List<string> activeStages, List<string> itemPhotoOfStages,
			List<DateTime> surveyDates, List<string> stageNameENs, List<string> stageNameVIs, List<string> weatherENs, List<string> weatherVIs,
			List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Tạm lưu lại dữ liệu cho view
				ViewData["ChooseIdMangrove"] = chooseIdMangrove;
				ViewData["IndexStages"] = indexStages;
				ViewData["ActiveStages"] = activeStages;
				ViewData["ItemPhotoOfStages"] = itemPhotoOfStages;

				ViewData["SurveyDates"] = surveyDates;
				ViewData["StageNameENs"] = stageNameENs;
				ViewData["StageNameVIs"] = stageNameVIs;
				ViewData["WeatherENs"] = weatherENs;
				ViewData["WeatherVIs"] = weatherVIs;

				ViewData["DataTypes"] = dataTypes;
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				// Tạo select
				var mangroves = await context.TblMangroves.ToListAsync();
				ViewData["Mangroves"] = mangroves;

				// Begin validate
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(model.Longitude);
				Helper.Validate.NotEmpty(model.Latitude);
				Helper.Validate.NotEmpty(model.PositionEn);
				Helper.Validate.NotEmpty(model.PositionVi);

				int indexPhoto = 0, indexNote = 0;
				for (int i = 0; i < indexStages.Count(); i++) {
					dataBase64s[indexPhoto] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[indexPhoto], dataTypes[indexPhoto]);
					Helper.Validate.NotEmpty(dataBase64s[indexPhoto]);

					Helper.Validate.NotEmpty(surveyDates[i].ToString("yyyy-MM-dd"));
					Helper.Validate.NotEmpty(stageNameENs[i]);
					Helper.Validate.NotEmpty(stageNameVIs[i]);
					Helper.Validate.NotEmpty(weatherENs[i], true);
					Helper.Validate.NotEmpty(weatherVIs[i], true);

					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						dataBase64s[j] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[j], dataTypes[j]);
						Helper.Validate.NotEmpty(dataBase64s[j]);
						Helper.Validate.NotEmpty(noteENs[indexNote]);
						Helper.Validate.NotEmpty(noteVIs[indexNote]);
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
				// Lưu cá thể
				string idIndividual = Helper.Func.CreateId();
				var newIndividual = new TblIndividual {
					Id = idIndividual,
					IdMangrove = chooseIdMangrove,
					Longitude = model.Longitude,
					Latitude = model.Latitude,
					PositionEn = model.PositionEn,
					PositionVi = model.PositionVi,
					UpdateLast = DateTime.Now,
					QrName = "qr-code.png", // Cần code tạo QR Image cho individual !
					View = 0
				};
				context.TblIndividuals.Add(newIndividual);

				// Truy vấn tên thành phần loài 
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == chooseIdMangrove);
				string nameMangrove = mangrove != null ? mangrove.NameVi : string.Empty;

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
						IdIndividual = idIndividual,
						MainImage = fileName,
						SurveyDay = surveyDates[i],
						NameEn = stageNameENs[i],
						NameVi = stageNameVIs[i],
						WeatherEn = weatherENs[i],
						WeatherVi = weatherVIs[i],
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
							NoteImgVi = noteVIs[indexNote]
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
				ViewData["Mangroves"] = await context.TblMangroves.ToListAsync();

				List<int> indexStages = new List<int>();
				List<string> activeStages = new List<string>();
				List<string> itemPhotoOfStages = new List<string>();
				List<string> idStages = new List<string>();

				List<DateTime> surveyDats = new List<DateTime>();
				List<string> stageNameENs = new List<string>();
				List<string> stageNameVIs = new List<string>();
				List<string> weatherENs = new List<string>();
				List<string> weatherVIs = new List<string>();

				List<string> dataBase64s = new List<string>();
				List<string> dataTypes = new List<string>();
				List<string> noteENs = new List<string>();
				List<string> noteVIs = new List<string>();

				for (int i = 0; i < individual.TblStages.Count(); i++) {
					indexStages.Add(i + 1);
					activeStages.Add(i == 0 ? "active" : string.Empty);

					var stage = individual.TblStages.ElementAt(i);

					var listPhotos = await context.TblPhotos.Where(item => item.IdObj == stage.Id).ToListAsync();
					itemPhotoOfStages.Add(listPhotos.Count().ToString());
					idStages.Add(stage.Id);

					surveyDats.Add(stage.SurveyDay);

					stageNameENs.Add(stage.NameEn);
					stageNameVIs.Add(stage.NameVi);
					weatherENs.Add(stage.WeatherEn ?? string.Empty);
					weatherVIs.Add(stage.WeatherVi ?? string.Empty);

					dataBase64s.Add(stage.MainImage);
					dataTypes.Add(string.Empty);
					foreach (var photo in listPhotos) {
						dataBase64s.Add(photo.ImageName);
						dataTypes.Add(string.Empty);
						noteENs.Add(photo.NoteImgEn ?? string.Empty);
						noteVIs.Add(photo.NoteImgVi ?? string.Empty);
					}
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
			List<DateTime> surveyDates, List<string> stageNameENs, List<string> stageNameVIs, List<string> weatherENs, List<string> weatherVIs,
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

				// Tạo select
				var mangroves = await context.TblMangroves.ToListAsync();
				ViewData["Mangroves"] = mangroves;

				// Begin validate
				Helper.Validate.Clear();
				Helper.Validate.NotEmpty(model.Longitude);
				Helper.Validate.NotEmpty(model.Latitude);
				Helper.Validate.NotEmpty(model.PositionEn);
				Helper.Validate.NotEmpty(model.PositionVi);

				int indexPhoto = 0, indexNote = 0;
				for (int i = 0; i < indexStages.Count(); i++) {
					dataBase64s[indexPhoto] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[indexPhoto], dataTypes[indexPhoto]);
					Helper.Validate.NotEmpty(dataBase64s[indexPhoto]);

					Helper.Validate.NotEmpty(surveyDates[i].ToString("yyyy-MM-dd"));
					Helper.Validate.NotEmpty(stageNameENs[i]);
					Helper.Validate.NotEmpty(stageNameVIs[i]);
					Helper.Validate.NotEmpty(weatherENs[i], true);
					Helper.Validate.NotEmpty(weatherVIs[i], true);

					int countItemPhotoOfStage = Convert.ToInt32(itemPhotoOfStages[i]);
					for (int j = indexPhoto + 1; j < indexPhoto + countItemPhotoOfStage + 1; j++) {
						dataBase64s[j] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[j], dataTypes[j]);
						Helper.Validate.NotEmpty(dataBase64s[j]);
						Helper.Validate.NotEmpty(noteENs[indexNote]);
						Helper.Validate.NotEmpty(noteVIs[indexNote]);
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
				// Cập nhật model
				context.TblIndividuals.Update(model);

				// Truy vấn tên thành phần loài 
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == chooseIdMangrove);
				string nameMangrove = mangrove != null ? mangrove.NameVi : string.Empty;

				List<string> saveIdStage = new List<string>();
				List<string> saveIdPhoto = new List<string>();

				indexPhoto = indexNote = 0;
				string fileName = string.Empty;
				for (int i = 0; i < indexStages.Count(); i++) {
					// Cập nhật giai đoạn
					string idStage = Helper.Func.CreateId();
					if (string.IsNullOrEmpty(idStages[i])) {
						fileName = $"{idStage}_{nameMangrove}{Helper.Func.GetTypeImage(dataTypes[indexPhoto])}";
						var newStage = new TblStage {
							Id = idStage,
							IdIndividual = model.Id,
							MainImage = fileName,
							SurveyDay = surveyDates[i],
							NameEn = stageNameENs[i],
							NameVi = stageNameVIs[i],
							WeatherEn = weatherENs[i],
							WeatherVi = weatherVIs[i],
						};
						context.TblStages.Add(newStage);
					}
					else {
						var oldStage = await context.TblStages.FirstOrDefaultAsync(item => item.Id == idStages[i]);
						if (oldStage != null) {
							idStage = oldStage.Id;

							oldStage.SurveyDay = surveyDates[i];
							oldStage.NameEn = stageNameENs[i];
							oldStage.NameVi = stageNameVIs[i];
							oldStage.WeatherEn = weatherENs[i];
							oldStage.WeatherVi = weatherVIs[i];

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
								IdObj = model.Id,
								ImageName = fileName,
								NoteImgEn = noteENs[indexNote],
								NoteImgVi = noteVIs[indexNote],
							};
							context.TblPhotos.Add(newPhoto);
						}
						else {
							fileName += Path.GetExtension(dataBase64s[j]);

							var photo = await context.TblPhotos.FindAsync(idPhoto);
							if (photo != null) {
								photo.ImageName = fileName;
								photo.NoteImgEn = noteENs[indexNote];
								photo.NoteImgVi = noteVIs[indexNote];
								context.TblPhotos.Update(photo);
							}
						}
						saveIdPhoto.Add(idPhoto);
						string newPath = Path.Combine(Helper.Path.stageImg, fileName);
						Helper.Func.MovePhoto(oldPath, newPath);
						indexNote += 1;
					}
					indexPhoto += countItemPhotoOfStage + 1;

					await context.SaveChangesAsync();

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

				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

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

							// Xoá giai đoạn này
							context.TblStages.Remove(stage);
						}
					}
				}
				await context.SaveChangesAsync();

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
