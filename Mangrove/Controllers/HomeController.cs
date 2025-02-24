using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
		// [Route("Result/{id}&{searchIndividual}")]
		public async Task<IActionResult> Result(string id, string? searchIndividual = null) {
			try {
				var mangrove = await context.TblMangroves.Include(o => o.TblIndividuals).FirstOrDefaultAsync(o => o.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<TblIndividual> listInidivuals;
					if (searchIndividual != null) {
						string search = searchIndividual.ToLower().Replace("/", "-");
						listInidivuals = mangrove.TblIndividuals
						.Where(o =>
							o.Position.ToLower().Contains(search) ||
							o.SurveyDay.ToString("dd/MM/yyyy").Contains(search)
						)
						.ToList();
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
				id = mangrove.Name;
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

				// individual.View += 1;
				// await context.SaveChangesAsync();

				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(o => o.Id == individual.IdMangrove);
				if (mangrove != null) {
					TempData["NameMangrove"] = mangrove.Name;
				}

				return View(individual);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Page: tìm kiếm
		public IActionResult Search() {
			return View();
		}
	}
}
