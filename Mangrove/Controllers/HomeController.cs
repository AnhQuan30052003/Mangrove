using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Mangrove.Controllers {
	public class HomeController : Controller {
		private readonly MangroveContext context;

		public HomeController(MangroveContext context) {
			this.context = context;
		}

		// Page: Xử lý xem có phải admin còn đăng nhập ?
		public async Task<IActionResult> Handle_Index() {
			try {
				var admin = HttpContext.User.FindFirst("Username")?.Value;
				// Có admin 
				if (!string.IsNullOrEmpty(admin)) {
					// Gia hạn thêm thời gian đăng nhập
					await HttpContext.SignInAsync(Helper.Variable.cookieName, HttpContext.User);

					// Chuyển đến trang thống kê - Quản trị viên
					return RedirectToAction("Page_Overview", "Statistical");
				}
				// Chuyển đến trang chủ - Người tra cứu
				return RedirectToAction("Page_Index");
			}
			catch {
				return RedirectToAction("Page_Index");
			}
		}

		// Get HomePage Client VM
		private async Task<HomePage_Client_VM> GetHomePageClient() {
			bool isEN = Helper.Func.IsEnglish();
			var home = await context.TblHomes.FirstOrDefaultAsync();
			var query = context.TblMangroves.OrderByDescending(item => item.UpdateLast);
			var listMangrove = await query.Take(home?.ItemRecent ?? 6).ToListAsync();

			// Truy vấn số item recent
			List<Mangrove_HomePage_Client_VM> list = new List<Mangrove_HomePage_Client_VM>();
			foreach (var mangrove in listMangrove) {
				var item = new Mangrove_HomePage_Client_VM {
					Id = mangrove.Id,
					Image = mangrove.MainImage,
					Name = (isEN ? mangrove.NameEn : mangrove.NameVi + " - " + mangrove.ScientificName)
				};
				list.Add(item);
			}

			// Truy vấn nhà tài trợ
			var sponsors = await context.TblPhotos
			.Where(item => item.IdObj == home!.Id)
			.OrderBy(item => item.NumberOrder)
			.ToListAsync();

			// Tạo Model
			var model = new HomePage_Client_VM {
				BannerTitle = (isEN ? home?.BannerTitleEn : home?.BannerTitleVi) ?? string.Empty,
				BannerImage = home?.BannerImg ?? "",
				PurposeTitle = (isEN ? home?.TitlePurposeEn : home?.TitlePurposeVi) ?? string.Empty,
				PurposeContent = (isEN ? home?.PurposeEn : home?.PurposeVi) ?? string.Empty,
				LabelSpeciesCompositionRecent = (isEN ? home?.TitleListItemEn : home?.TitleListItemVi) ?? string.Empty,
				Mangroves = list,
				Sponsor = sponsors
			};

			return model;
		}

		// Page: Trang chủ
		public async Task<IActionResult> Page_Index() {
			try {
				var model = await GetHomePageClient();
				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
			}
		}

		// Kết quả tìm kiếm cây
		public async Task<IActionResult> Page_Result(string id, string? searchIndividual = null) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var mangrove = await context.TblMangroves
				.Include(item => item.TblIndividuals)
				.ThenInclude(item => item.TblStages)
				.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				// Danh sách cá thể
				List<Individual_Mangrove_Client_VM> listIndividual = new List<Individual_Mangrove_Client_VM>();
				foreach (var indi in mangrove.TblIndividuals) {
					var item = new Individual_Mangrove_Client_VM {
						Id = indi.Id,
						UpdateLast = indi.UpdateLast.ToString("dd/MM/yyyy").Replace("-", "/"),
						View = indi.View,
						NumberStages = indi.TblStages.Count(),
						Position = isEN ? indi.PositionEn : indi.PositionVi,
						QrName = indi.QrName,
						NameMangrove = isEN ? mangrove.NameEn : $"{mangrove.NameVi} - {mangrove.ScientificName}",
						Longitude = indi.Longitude ?? string.Empty,
						Latitude = indi.Latitude ?? string.Empty
					};
					listIndividual.Add(item);
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
						if (photo.Image == mangrove.MainImage) {
							var save = photo;
							listPhoto.Remove(save);
							listPhoto.Insert(0, save);
							break;
						}
					}
				}

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<Individual_Mangrove_Client_VM> fillter = listIndividual;
					if (!string.IsNullOrEmpty(searchIndividual)) {
						string search = searchIndividual.Replace("/", "-");

						fillter = new List<Individual_Mangrove_Client_VM>();
						foreach (var indi in mangrove.TblIndividuals.ToList()) {
							var conditions = new List<string>();
							conditions.Add(indi.UpdateLast.ToString("dd/MM/yyyy"));
							conditions.Add(indi.PositionEn);
							conditions.Add(indi.PositionVi);

							if (Helper.Func.CheckContain(search, conditions)) {
								var item = new Individual_Mangrove_Client_VM {
									Id = indi.Id,
									UpdateLast = indi.UpdateLast.ToString("dd/MM/yyyy").Replace("-", "/"),
									View = indi.View,
									NumberStages = indi.TblStages.Count(),
									Position = isEN ? indi.PositionEn : indi.PositionVi,
									QrName = indi.QrName
								};
								fillter.Add(item);
							}
						}
					}

					return PartialView($"{Helper.Path.partialView}/User_Individuals.cshtml", fillter);
				}

				// Tăng view của cây
				string cookieIdView = $"Mangrove_{id}";
				if (!Request.Cookies.ContainsKey(cookieIdView)) {
					// Tăng View vì chưa xem
					mangrove.View += 1;
					await context.SaveChangesAsync();

					// Sau đó đợi 10 phút mới cho trình duyệt cập nhật lại
					HttpContext.Response.Cookies.Append(cookieIdView, "Viewed", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMinutes(10) });
				}

				var model = new Mangrove_Client_VM {
					Id = mangrove.Id,
					MainImage = mangrove.MainImage,
					Name = isEN ? mangrove.NameEn : mangrove.NameVi,
					CommonName = isEN ? mangrove.CommonNameEn : mangrove.CommonNameVi,
					ScientificName = mangrove.ScientificName,
					Familia = mangrove.Familia,
					Use = isEN ? mangrove.UseEn : mangrove.UseVi,
					Morphology = isEN ? mangrove.MorphologyEn : mangrove.MorphologyVi,
					Ecology = isEN ? mangrove.EcologyEn : mangrove.EcologyVi,
					TitleDistribution = (isEN ? mangrove.TitleDistributionEn : mangrove.TitleDistributionVi) ?? string.Empty,
					Distribution = isEN ? mangrove.DistributionEn : mangrove.DistributionVi,
					ConservationStatus = isEN ? mangrove.ConservationStatusEn : mangrove.ConservationStatusVi,
					View = mangrove.View,
					Photos = listPhoto,
					Individuals = listIndividual
				};

				return View(model);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The mangrove website you just accessed has an error. Please try again later !" : "Website cây ngập mặn vừa truy cập bị lỗi. Hãy truy cập lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
		}

		// Page: cá thể của cây
		public async Task<IActionResult> Page_Individual(string id) {
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

				// Tăng View cho lần xem này
				string cookieIdView = $"Individual_{id}";
				if (!Request.Cookies.ContainsKey(cookieIdView)) {
					// Tăng View vì chưa xem
					individual.View += 1;
					await context.SaveChangesAsync();

					// Sau đó đợi 10 phút mới cho trình duyệt cập nhật lại
					HttpContext.Response.Cookies.Append(cookieIdView, "Viewed", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMinutes(10) });
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
					isEN ? "The mangrove individual website you just accessed has an error. Please try again later !" : "Website cá thể cây ngập mặn vừa truy cập bị lỗi. Hãy truy cập lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
		}

		// Page: thành phần loài - có tìm kiếm
		public async Task<IActionResult> Page_SpeciesComposition(string? search = null) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				List<TblMangrove> listMangrove = isEN ? await context.TblMangroves.OrderBy(item => item.NameEn).ToListAsync() : await context.TblMangroves.OrderBy(item => item.NameVi).ToListAsync();

				var model = new List<SearchMangroe_Client_VM>();
				foreach (var mangrove in listMangrove) {
					var item = new SearchMangroe_Client_VM {
						Id = mangrove.Id,
						Name = isEN ? mangrove.NameEn : mangrove.NameVi + " - " + mangrove.ScientificName,
						Morphology = isEN ? mangrove.MorphologyEn : mangrove.MorphologyVi
					};
					model.Add(item);
				}

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					var fillter = model;
					if (!string.IsNullOrEmpty(search)) {
						fillter = new List<SearchMangroe_Client_VM>();
						foreach (var mangrove in listMangrove) {
							var conditions = new List<string>();
							conditions.Add(mangrove.NameVi);
							conditions.Add(mangrove.NameEn);

							if (Helper.Func.CheckContain(search, conditions)) {
								var item = new SearchMangroe_Client_VM {
									Id = mangrove.Id,
									Name = isEN ? mangrove.NameEn : mangrove.NameVi + " - " + mangrove.ScientificName,
									Morphology = isEN ? mangrove.MorphologyEn : mangrove.MorphologyVi
								};
								fillter.Add(item);
							}
						}
					}

					return PartialView($"{Helper.Path.partialView}/User_ListMangrove.cshtml", fillter);
				}

				return View(model);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The mangrove tree search website is down. Please try again later !" : "Website tìm kiếm cây ngập mặn bị lỗi. Hãy truy cập lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
		}

		// Page: phân bố
		public async Task<IActionResult> Page_Distribution(string showType = "") {
			showType = showType.Trim();
			showType = string.IsNullOrEmpty(showType) ? Helper.Key.showList : showType;
			ViewData["ShowType"] = showType;

			bool isEN = Helper.Func.IsEnglish();
			try {
				var model = await context.TblDistributitons
				.Select(item => new Distribution_Client_VM {
					Image = item.ImageMap,
					Position = isEN ? item.MapNameEn : item.MapNameVi
				})
				.ToListAsync();

				return View(model);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The mangrove distribution map website is down. Please try again later !" : "Website bản đồ phân bố cây ngập mặn bị lỗi. Hãy truy cập lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
		}

		// Thông tin chung user
		public async Task<IActionResult> Page_OverviewMangrove() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var info = await context.TblInforOverviews.FirstOrDefaultAsync();
				List<TblPhoto> listPhoto = new List<TblPhoto>();

				// Danh sách hình ảnh
				if (info != null) {
					listPhoto = await context.TblPhotos
					.Where(item => item.IdObj == info.Id)
					.OrderBy(item => item.NumberOrder)
					.ToListAsync();
				}

				ViewData["Photos"] = listPhoto;

				return View(info);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "The mangrove overview website is currently unavailable. Please try again later !" : "Trang web tổng quan về rừng ngập mặn hiện không khả dụng. Vui lòng thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_Index");
			}
		}

		[Authorize]
		// Page: View home (admin)
		public async Task<IActionResult> Page_IndexAdmin_View() {
			try {
				var model = await GetHomePageClient();
				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
			}
		}

		[Authorize]
		// Page: Edit home (admin)
		public async Task<IActionResult> Page_IndexAdmin_Edit() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var home = await context.TblHomes.FirstOrDefaultAsync(item => item.Id == "H0000000-AAAA-AAAA-AAAA-AAAAAAAA0000");
				if (home == null) {
					return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.notExists });
				}

				var sponsors = await context.TblPhotos
				.Where(item => item.IdObj == home.Id)
				.OrderBy(item => item.NumberOrder)
				.ToListAsync();

				// Tạo list dữ liệu
				var dataBase64s = new List<string>();
				var dataTypes = new List<string>();
				var NoteENs = new List<string>();
				var NoteVIs = new List<string>();

				dataBase64s.Add(home.BannerImg);
				dataTypes.Add(string.Empty);
				NoteENs.Add(home.BannerTitleEn);
				NoteVIs.Add(home.BannerTitleVi);

				foreach (var sponsor in sponsors) {
					dataBase64s.Add(sponsor.ImageName);
					dataTypes.Add(string.Empty);
					NoteENs.Add(sponsor.NoteImgEn ?? string.Empty);
					NoteVIs.Add(sponsor.NoteImgVi ?? string.Empty);
				}

				ViewData["DataBase64s"] = dataBase64s;
				ViewData["DataTypes"] = dataTypes;
				ViewData["NoteENs"] = NoteENs;
				ViewData["NoteVIs"] = NoteVIs;

				Helper.Validate.Clear();
				return View(home);
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Request to access edit status failed. Please try again later !" : "Gửi yêu cầu truy cập trang chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return RedirectToAction("Page_IndexAdmin_View");
			}
		}
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Page_IndexAdmin_Edit(TblHome model, List<string> dataTypes, List<string> dataBase64s, List<string> noteENs, List<string> noteVIs) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				ViewData["DataBase64s"] = dataBase64s;
				ViewData["DataTypes"] = dataTypes;
				ViewData["NoteENs"] = noteENs;
				ViewData["NoteVIs"] = noteVIs;

				// Begin validate
				Helper.Validate.Clear();
				for (int i = 0; i < dataBase64s.Count(); i++) {
					dataBase64s[i] = await Helper.Func.CheckIsDataBase64StringAndSave(dataBase64s[i], dataTypes[i]);
					Helper.Validate.NotEmpty(dataBase64s[i]);

					if (i == 0) {
						Helper.Validate.MaxLength(noteENs[i], 256, false);
						Helper.Validate.MaxLength(noteVIs[i], 256, false);
					}
					else {
						Helper.Validate.NotEmpty(noteENs[i], true);
						Helper.Validate.NotEmpty(noteVIs[i], true);
					}
				}

				Helper.Validate.MaxLength(model.TitlePurposeEn, 256);
				Helper.Validate.MaxLength(model.TitlePurposeVi, 256);
				Helper.Validate.NotEmpty(model.PurposeEn);
				Helper.Validate.NotEmpty(model.PurposeVi);
				Helper.Validate.MaxLength(model.TitleListItemEn, 256);
				Helper.Validate.MaxLength(model.TitleListItemVi, 256);
				Helper.Validate.NotEmpty(model.ItemRecent.ToString());

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					Helper.Notifier.Fail(
						isEN ? "Some input fields are missing or contain errors !" : " Một số ô nhập liệu còn thiếu hoặc chứa lỗi !",
						Helper.SetupNotifier.Timer.shortTime
					);
					return View(model);
				}
				// End valiate

				// Save data
				// Save banner khi có ảnh mới
				if (dataBase64s[0].Contains(Helper.Key.temp)) {
					Helper.Func.DeletePhoto(Helper.Path.logo, model.BannerImg);
					string bannerName = $"banner{Helper.Func.GetTypeImage(dataTypes[0])}";
					Helper.Func.MovePhoto(
						Path.Combine(Helper.Path.temptImg, dataBase64s[0]),
						Path.Combine(Helper.Path.logo, bannerName)
					);
					model.BannerImg = bannerName;
				}

				model.BannerTitleEn = noteENs[0];
				model.BannerTitleVi = noteVIs[0];
				context.TblHomes.Update(model);

				// Save sponsor
				List<string> saveIdSponsor = new List<string>();
				for (int i = 1; i < dataBase64s.Count(); i++) {
					// Nếu có ảnh mới	
					string idSponsor = Helper.Func.CreateId();
					if (dataBase64s[i].Contains(Helper.Key.temp)) {
						string fileName = $"{idSponsor}_{noteVIs[i]}{Helper.Func.GetTypeImage(dataTypes[i])}";
						Helper.Func.MovePhoto(
							Path.Combine(Helper.Path.temptImg, dataBase64s[i]),
							Path.Combine(Helper.Path.sponImg, fileName)
						);

						var newSponsor = new TblPhoto {
							Id = idSponsor,
							IdObj = model.Id,
							ImageName = fileName,
							NoteImgVi = noteVIs[i],
							NoteImgEn = noteENs[i],
							NumberOrder = i - 1
						};
						context.TblPhotos.Add(newSponsor);
					}
					else {
						idSponsor = Helper.Func.GetIdFromFileName(dataBase64s[i]);
						var oldSponsor = await context.TblPhotos.FirstOrDefaultAsync(item => item.Id == idSponsor);
						if (oldSponsor != null) {
							string fileName = $"{idSponsor}_{noteVIs[i]}{Path.GetExtension(dataBase64s[i])}";
							Helper.Func.MovePhoto(
								Path.Combine(Helper.Path.sponImg, dataBase64s[i]),
								Path.Combine(Helper.Path.sponImg, fileName)
							);

							oldSponsor.ImageName = fileName;
							oldSponsor.NoteImgVi = noteVIs[i];
							oldSponsor.NoteImgEn = noteENs[i];
							oldSponsor.NumberOrder = i - 1;
							context.TblPhotos.Update(oldSponsor);
						}
					}
					saveIdSponsor.Add(idSponsor);
				}

				// Xóa sponsor không còn tồn tại
				var sponsorDelete = await context.TblPhotos.Where(item => item.IdObj == model.Id).ToListAsync();
				foreach (var sponsor in sponsorDelete) {
					if (!saveIdSponsor.Contains(sponsor.Id)) {
						Helper.Func.DeletePhoto(Helper.Path.sponImg, sponsor.ImageName);
						context.TblPhotos.Remove(sponsor);
					}
				}

				await context.SaveChangesAsync();
				Helper.Func.DeleteAllFile(Helper.Path.temptImg);

				// Setup thông báo thành công 
				Helper.Notifier.Success(
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime
				);
				return RedirectToAction("Page_IndexAdmin_View");
			}
			catch {
				Helper.Notifier.Fail(
					isEN ? "Edit request failed. Please try again later !" : "Yêu cầu chỉnh sửa thất bại. Hãy thử lại sau !",
					Helper.SetupNotifier.Timer.midTime
				);
				return View(model);
			}
		}
	}
}
