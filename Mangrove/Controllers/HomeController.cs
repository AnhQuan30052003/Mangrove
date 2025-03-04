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

		// Thay đổi language với ajax
		public IActionResult SaveChangeLanguage(string lang) {
			HttpContext.Session.SetString("language", lang);
			Console.WriteLine($"Save new language after change is: {lang}");

			return NoContent();
		}

		// Page: Trang chủ
		public async Task<IActionResult> Page_Index() {
			try {
				// Truy vấn 6 item gần đây nhất
				int quantityShow = 6;
				var mangroves = await context.TblMangroves
				.OrderByDescending(item => item.UpdateLast)
				.ThenBy(item => item.Name)
				.Take(quantityShow)
				.ToListAsync();

				return View(mangroves);
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
				.FirstOrDefaultAsync(item => item.Id == id.ToUpper());
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<TblIndividual> listInidivuals;
					if (!string.IsNullOrEmpty(searchIndividual)) {
						string search = searchIndividual.ToLower().Replace("/", "-");
						listInidivuals = mangrove.TblIndividuals.Where(o =>
							o.Position.ToLower().Contains(search) ||
							o.UpdateLast.ToString("dd/MM/yyyy").Contains(search)
						).ToList();
					}
					else listInidivuals = mangrove.TblIndividuals.ToList();

					return PartialView($"{Helper.Path.partialView}/User_Individuals.cshtml", listInidivuals);
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
					NameMangrove = mangrove?.Name + " - " + mangrove?.ScientificName,
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
		public async Task<IActionResult> Page_SpeciesComposition(string? search = null) { // * Cần fix chuyển đổi ngôn ngữ khi dịch => Cần tìm cách tối ưu hơn để dịch nhanh + tốt hơn
			try {
				List<TblMangrove> listMangrove = await context.TblMangroves.OrderBy(o => o.Name).ToListAsync();

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					if (!string.IsNullOrEmpty(search)) {
						search = search.ToLower();
						string unsignStringSearch = Helper.Func.FormatUngisnedString(search);

						var translatedList = await Task.WhenAll(listMangrove.Select(async item => new {
							Item = item,
							NameVI = await Helper.Func.Translate(item.Name.ToLower(), "en", "vi"),
							NameEN = await Helper.Func.Translate(item.Name.ToLower(), "vi", "en")
						}));

						listMangrove = translatedList
						.Where(x =>
							x.NameVI.Contains(search) || Helper.Func.FormatUngisnedString(x.NameVI).Contains(unsignStringSearch) ||
							x.NameEN.Contains(search) || x.Item.ScientificName.ToLower().Contains(search)
						)
						.Select(x => x.Item)
						.ToList();

						// var data = listMangrove;
						// listMangrove = new List<TblMangrove>();

						// foreach (var item in data) {
						// 	// Tìm kiểm ở Tiếng Việt
						// 	string nameTranslate = await Helper.Func.Translate(item.Name.ToLower(), "en", "vi");
						// 	bool searchVI = nameTranslate.Contains(search) || Helper.Func.FormatUngisnedString(nameTranslate).Contains(unsignStringSearch);

						// 	// Tìm kiếm ở Tiếng Anh
						// 	nameTranslate = await Helper.Func.Translate(item.Name.ToLower(), "vi", "en");
						// 	bool searchEN = nameTranslate.Contains(search) || item.ScientificName.ToLower().Contains(search);

						// 	if (searchVI || searchEN) listMangrove.Add(item);
						// }
					}

					return PartialView($"{Helper.Path.partialView}/User_ListMangrove.cshtml", listMangrove);
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
