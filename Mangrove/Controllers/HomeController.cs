using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Configuration;
using System.Net.Mime;

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
				var mangroves = await query.Take(home?.ItemRecent ?? 6).ToListAsync();
				TempData["Mangroves"] = mangroves;

				return View(home);
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
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<TblIndividual> fillter = mangrove.TblIndividuals.ToList();
					if (!string.IsNullOrEmpty(searchIndividual)) {
						string search = searchIndividual.ToLower().Replace("/", "-");

						fillter = new List<TblIndividual>();
						foreach (var item in mangrove.TblIndividuals) {
							if (item.UpdateLast.ToString("dd/MM/yyyy").Contains(search) ||
								item.PositionEn.ToLower().Contains(search) ||
								item.PositionVi.ToLower().Contains(search)
							) {
								fillter.Add(item);
							}
						}
					}

					return PartialView($"{Helper.Path.partialView}/User_Individuals.cshtml", fillter);
				}

				// Truy vấn ảnh cho banner slick slider
				var photos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				var photoMangrove = await context.TblPhotos.FirstOrDefaultAsync(item => item.ImageName == mangrove.MainImage);

				// Xử lý thứ tự ảnh banner slick slider
				if (photos.Count() > 1 && photoMangrove != null) {
					photos.Remove(photoMangrove);
					photos.Insert(0, photoMangrove);
				}

				// Tăng view của cây
				mangrove.View += 1;
				await context.SaveChangesAsync();

				TempData["Photos"] = photos;
				return View(mangrove);
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
					return NotFound($"Không tìm thấy cá thể có ID: {id}");
				}

				individual.View += 1;
				await context.SaveChangesAsync();

				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(o => o.Id == individual.IdMangrove);

				// Truy vấn giai đoạn và thông tin mỗi giai đoạn
				List<Stage> listStages = new List<Stage>();
				foreach (var stage in individual.TblStages) {
					var photos = await context.TblPhotos.Where(item => item.IdObj == stage.Id).ToListAsync();
					if (photos.Count == 0) {
						photos = new List<TblPhoto>();
					}
					else {
						var photo = await context.TblPhotos.FirstOrDefaultAsync(item => item.IdObj == stage.Id);
						if (photo != null) {
							photos.Remove(photo);
						}
					}

					var _stage = new Stage {
						info = stage,
						photo = photos
					};

					listStages.Add(_stage);
				}

				var info = new InfoStagesOfIndividualModel {
					NameMangrove = (Helper.Func.IsLanguage("en") ? mangrove?.NameEn : mangrove?.NameVi + " - " + mangrove?.ScientificName) ?? "",
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
				bool isEN = Helper.Func.IsLanguage("en");
				List<TblMangrove> listMangrove = isEN ? await context.TblMangroves.OrderBy(o => o.NameEn).ToListAsync() : await context.TblMangroves.OrderBy(o => o.NameVi).ToListAsync();

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<TblMangrove> fillter = listMangrove;
					if (!string.IsNullOrEmpty(search)) {
						search = search.ToLower();
						string unsignStringSearch = Helper.Func.FormatUngisnedString(search);

						fillter = new List<TblMangrove>();
						foreach (var item in listMangrove) {
							if (item.NameVi.ToLower().Contains(search) || Helper.Func.FormatUngisnedString(item.NameVi.ToLower()).Contains(unsignStringSearch) || item.NameEn.ToLower().Contains(search)) {
								fillter.Add(item);
							}
						}
					}

					return PartialView($"{Helper.Path.partialView}/User_ListMangrove.cshtml", fillter);
				}

				return View(listMangrove);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: phân bố
		public IActionResult Page_Distribution() {

			return View();
		}
	}
}
