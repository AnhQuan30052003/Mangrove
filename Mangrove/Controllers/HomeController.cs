using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Configuration;
using System.Net.Mime;
using Mangrove.ViewModels;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Mangrove.Controllers {
	public class HomeController : Controller {
		private readonly MangroveContext context;

		public HomeController(MangroveContext context) {
			this.context = context;
		}

		// Page: Xử lý xem có phải admin còn đăng nhập ?
		public IActionResult Handle_Index() {
			try {
				var admin = HttpContext.User.FindFirst("Username")?.Value;
				// Có admin 
				if (!string.IsNullOrEmpty(admin)) {
					return RedirectToAction("Page_Overview", "Statistical");
				}
				return RedirectToAction("Page_Index");
			}
			catch {
				return RedirectToAction("Page_Index");
			}
		}

		// Page: Trang chủ
		public async Task<IActionResult> Page_Index() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var home = await context.TblHomes.FirstOrDefaultAsync();
				var query = context.TblMangroves.OrderByDescending(item => item.UpdateLast);
				var listMangrove = await query.Take(home?.ItemRecent ?? 6).ToListAsync();

				List<Mangrove_HomePage_Client_VM> list = new List<Mangrove_HomePage_Client_VM>();
				foreach (var mangrove in listMangrove) {
					var item = new Mangrove_HomePage_Client_VM {
						Id = mangrove.Id,
						Image = mangrove.MainImage,
						Name = (isEN ? mangrove.NameEn : mangrove.NameVi + " - " + mangrove.ScientificName)
					};
					list.Add(item);
				}

				var model = new HomePage_Client_VM {
					BannerTitle = (isEN ? home?.BannerTitleEn : home?.BannerTitleVi) ?? "",
					BannerImage = home?.BannerImg ?? "",
					Purpose = (isEN ? home?.PurposeEn : home?.PurposeVi) ?? "",
					Mangroves = list,
				};

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
				mangrove.View += 1;
				await context.SaveChangesAsync();

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
				individual.View += 1;
				await context.SaveChangesAsync();

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
		public async Task<IActionResult> Page_Distribution() {
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

		// Page: View home (admin)
		public async Task<IActionResult> Page_IndexAdmin_View() {
			bool isEN = Helper.Func.IsEnglish();
			try {
				var home = await context.TblHomes.FirstOrDefaultAsync();
				var query = context.TblMangroves.OrderByDescending(item => item.UpdateLast);
				var listMangrove = await query.Take(home?.ItemRecent ?? 6).ToListAsync();

				List<Mangrove_HomePage_Client_VM> list = new List<Mangrove_HomePage_Client_VM>();
				foreach (var mangrove in listMangrove) {
					var item = new Mangrove_HomePage_Client_VM {
						Id = mangrove.Id,
						Image = mangrove.MainImage,
						Name = (isEN ? mangrove.NameEn : mangrove.NameVi + " - " + mangrove.ScientificName)
					};
					list.Add(item);
				}

				var model = new HomePage_Client_VM {
					BannerTitle = (isEN ? home?.BannerTitleEn : home?.BannerTitleVi) ?? "",
					BannerImage = home?.BannerImg ?? "",
					Purpose = (isEN ? home?.PurposeEn : home?.PurposeVi) ?? "",
					Mangroves = list,
				};

				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return RedirectToAction("Page_Error", "SettingWebsite", new { typeError = Helper.Variable.TypeError.disconnectDatabase });
			}
		}

	}
}
