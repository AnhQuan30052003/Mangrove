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

		public async Task<IActionResult> Result(string id, string? searchInvidiual = null) {
			try {
				//GetDistanceYear();

				var mangrove = await context.TblMangroves.Include(o => o.TblIndividuals).FirstOrDefaultAsync(o => o.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}
				else {
					var photos = await context.TblPhotos.Where(o => o.IdObj == id).ToListAsync();
					var photoMangrove = await context.TblPhotos.Where(o => o.ImageName == mangrove.MainImage).FirstOrDefaultAsync();

					if (photoMangrove != null) {
						photos.Remove(photoMangrove);
						photos.Insert(0, photoMangrove);
					}

					mangrove.View += 1;
					await context.SaveChangesAsync();

					// Code Ajax tìm cá thể
					// if (Request.Headers["REQUESTED"] == "AJAX" && searchInvidiual != null) {
					// 	// Xử lý logic tìm kiếm

					// 	DateTime searchDate = DateTime.ParseExact(searchInvidiual, "dd/MM/yyyy", CultureInfo.InvariantCulture);
					// 	string searchString = searchInvidiual.ToLower();

					// 	List<TblIndividual> listInidivuals = mangrove.TblIndividuals
					// 	.Where(o => o.SurveyDay.ToString("dd/MM/yyyy").Contains("") || o.Position.Contains(searchString))
					// 	.ToList();




					// 	return PartialView($"{Helper.Path.partialView}/Individuals.cshtml", listInidivuals);
					// }

					TempData["Photos"] = photos;
					TempData["ListIndividuals"] = mangrove.TblIndividuals.ToList();
					Console.WriteLine($"Số lượng item của {mangrove.Name} là: {mangrove.TblIndividuals.ToList().Count()}");
					return View(mangrove);
				}
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
