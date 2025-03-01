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
		public async Task<IActionResult> Index() {
			try {
				// Truy vấn 6 item gần đây nhất
				int quantityShow = 6;
				var mangroves = await context.TblMangroves
				.Skip(0)
				.Take(quantityShow)
				.OrderByDescending(o => o.UpdateLast)
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
		public async Task<IActionResult> Result(string id = "", string? searchIndividual = null) {
			try {
				var mangrove = await context.TblMangroves
				.Include(o => o.TblIndividuals)
				.ThenInclude(o => o.TblStages)
				.FirstOrDefaultAsync(o => o.Id == id.ToUpper());
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

					return PartialView($"{Helper.Path.partialView}/Individuals.cshtml", listInidivuals);
				}

				// Truy vấn ảnh cho banner slick slider
				var photos = await context.TblPhotos.Where(o => o.IdObj == id).ToListAsync();
				var photoMangrove = await context.TblPhotos.FirstOrDefaultAsync(o => o.ImageName == mangrove.MainImage);

				// Xử lý thứ tự ảnh banner slick slider
				if (photos.Count() > 1 && photoMangrove != null) {
					photos.Remove(photoMangrove);
					photos.Insert(0, photoMangrove);
				}

				// Tăng view của cây
				mangrove.View += 1;
				await context.SaveChangesAsync();

				TempData["Photos"] = photos;
				TempData["ListIndividuals"] = mangrove.TblIndividuals.ToList();
				return View(mangrove);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: cá thể của cây
		public async Task<IActionResult> Individual(string id) {
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
					var photos = await context.TblPhotos.Where(o => o.IdObj == stage.Id).ToListAsync();
					if (photos.Count == 0) {
						photos = new List<TblPhoto>();
					}
					else {
						var photo = await context.TblPhotos.FirstOrDefaultAsync(o => o.IdObj == stage.Id);
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
		public async Task<IActionResult> SpeciesComposition(string? search = null) {
			try {
				List<TblMangrove> listMangrove;

				var query = context.TblMangroves
				.OrderBy(o => o.Name)
				.AsQueryable();

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					if (!string.IsNullOrEmpty(search)) {
						search = search.ToLower();

						listMangrove = await query
						.Where(o =>
							o.Name.ToLower().Contains(search) ||
							o.ScientificName.ToLower().Contains(search) ||
							o.Morphology.ToLower().Contains(search)
						// o.OtherName.ToLower().Contains(search) ||
						)
						.ToListAsync();
					}
					else listMangrove = await query.ToListAsync();
					return PartialView($"{Helper.Path.partialView}/ListMangrove.cshtml", listMangrove);
				}
				else listMangrove = await query.ToListAsync();

				return View(listMangrove);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: phân bố
		public IActionResult Distribution() {

			return View();
		}
	}
}
