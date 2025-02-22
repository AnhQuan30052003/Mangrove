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
				//GetDistanceYear();
				//TempData["Status"] = Helper.StatusNoifier.success;
				//TempData["Content"] = "Thành công test code";

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
				Console.WriteLine("Error: " + ex.Message);
				return NotFound("Có lỗi khi kết nối với Cơ sở dữ liệu");
			}
		}

		// Page: Kết quả tìm kiếm ?
		public IActionResult Results() {
			//GetDistanceYear();

			return View();
		}

		public async Task<IActionResult> Result(string id, string? searchIndividual = null) {
			try {
				var mangrove = await context.TblMangroves.Include(o => o.TblIndividuals).FirstOrDefaultAsync(o => o.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}

				// Truy vấn ảnh cho banner slick slider
				var photos = await context.TblPhotos.Where(o => o.IdObj == id).ToListAsync();
				var photoMangrove = await context.TblPhotos.Where(o => o.ImageName == mangrove.MainImage).FirstOrDefaultAsync();

				// Code Ajax tìm cá thể
				if (Request.Headers["REQUESTED"] == "AJAX") {
					// Xử lý logic tìm kiếm
					List<TblIndividual> listInidivuals;
					if (searchIndividual != null) {
						listInidivuals = mangrove.TblIndividuals
						.Where(o =>
							o.Position.ToLower().Contains(searchIndividual.ToLower()) ||
							o.SurveyDay.ToString("dd/MM/yyyy").Replace("-", "/").Contains(searchIndividual.ToLower().Replace("-", "/"))
						)
						.ToList();
					}
					else listInidivuals = mangrove.TblIndividuals.ToList();

					return PartialView($"{Helper.Path.partialView}/Individuals.cshtml", listInidivuals);
				}

				// Xử lý thứ tự ảnh banner slick slider
				if (photoMangrove != null) {
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
				Console.WriteLine("Error: " + ex.Message);
				return NotFound("Có lỗi khi kết nối với Cơ sở dữ liệu");
			}
		}

		// Page: cá thể của cây
		public IActionResult Individual(string id) {
			return View();
		}

		// Page: tìm kiếm
		public IActionResult Search() {
			return View();
		}

		// Hàm riêng
		// Truy vấn thời gian (năm) chi filter tìm kiếm
		// private async void GetDistanceYear() {
		// 	try {
		// 		var home = await context.TblHomes.FirstOrDefaultAsync();
		// 		if (home == null) {
		// 			Console.WriteLine("Bảng Home không có dữ liệu");
		// 		}
		// 		else {
		// 			TempData["YearStart"] = home.YearStart;
		// 			TempData["YearEnd"] = DateTime.Now.Year;
		// 		}
		// 	}
		// 	catch (Exception ex) {
		// 		Console.WriteLine("Không kết nối được với Cơ sở dữ liệu");
		// 		Console.WriteLine("Error: " + ex.Message);
		// 	}
		// }
	}
}
