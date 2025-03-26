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

namespace Mangrove.Controllers {
	public class HomeController : Controller {
		private readonly MangroveContext context;

		public HomeController(MangroveContext context) {
			this.context = context;
		}

		// Page: Trang chủ
		public async Task<IActionResult> Page_Index() {
			try {
				var home = await context.TblHomes.FirstOrDefaultAsync();
				var query = context.TblMangroves.OrderByDescending(item => item.UpdateLast);
				var listMangrove = await query.Take(home?.ItemRecent ?? 6).ToListAsync();

				bool isEN = Helper.Func.IsEnglish();
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
				return NotFound(notifier);
			}
		}

		// Kết quả tìm kiếm cây
		public async Task<IActionResult> Page_Result(string id = "", string? searchIndividual = null) {
			try {
				var mangrove = await context.TblMangroves
				.Include(item => item.TblIndividuals)
				.ThenInclude(item => item.TblStages)
				.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					return RedirectToAction("Page_Index");
				}

				bool isEN = Helper.Func.IsEnglish();

				// Danh sách cá thể
				List<Individual_Mangrove_Client_VM> listIndividual = new List<Individual_Mangrove_Client_VM>();
				foreach (var indi in mangrove.TblIndividuals) {
					var item = new Individual_Mangrove_Client_VM {
						Id = indi.Id,
						UpdateLast = indi.UpdateLast.ToString("dd/MM/yyyy").Replace("-", "/"),
						View = indi.View,
						NumberStages = indi.TblStages.Count(),
						Position = isEN ? indi.PositionEn : indi.PositionVi,
						QrName = indi.QrName
					};
					listIndividual.Add(item);
				}

				// Danh sách hình ảnh
				List<Photo_Mangrove_Client_VM> listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == id)
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
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: cá thể của cây
		public async Task<IActionResult> Page_Individual(string id) {
			try {
				var individual = await context.TblIndividuals.Include(o => o.TblStages).FirstOrDefaultAsync(o => o.Id == id);
				if (individual == null) {
					return RedirectToAction("Page_Index");
				}

				individual.View += 1;
				await context.SaveChangesAsync();

				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(o => o.Id == individual.IdMangrove);

				// Truy vấn giai đoạn và thông tin mỗi giai đoạn
				List<Stage> listStages = new List<Stage>();
				foreach (var stage in individual.TblStages.ToList()) {
					var photos = await context.TblPhotos.Where(item => item.IdObj == stage.Id).ToListAsync();
					if (photos == null || photos.Count() == 0) {
						photos = new List<TblPhoto>();
					}

					var _stage = new Stage {
						info = stage,
						photo = photos
					};

					listStages.Add(_stage);
				}

				var info = new InfoStagesOfIndividual_Client_VM {
					NameMangrove = (Helper.Func.IsEnglish() ? mangrove?.NameEn : mangrove?.NameVi + " - " + mangrove?.ScientificName) ?? "",
					Individual = individual,
					Stages = listStages
				};

				return View(info);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: thành phần loài - có tìm kiếm
		public async Task<IActionResult> Page_SpeciesComposition(string? search = null) {
			try {
				bool isEN = Helper.Func.IsEnglish();
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
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: phân bố
		public async Task<IActionResult> Page_Distribution() {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var model = await context.TblDistributitons
				.Select(item => new Distribution_Client_VM {
					Image = item.ImageMap,
					Position = isEN ? item.MapNameEn : item.MapNameVi
				})
				.ToListAsync();

				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
	}
}
