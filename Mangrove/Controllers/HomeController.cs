using System.Diagnostics;
using System.Threading.Tasks;
using Mangrove.Data;
using Mangrove.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mangrove.Controllers {
	public class HomeController : Controller {
		private readonly MangroveContext context;

		public HomeController(MangroveContext context) {
			this.context = context;
		}

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
				return NotFound("Không kết nối được với Cơ sở dữ liệu");
			}
		}

		public IActionResult Results() {
			//GetDistanceYear();

			return View();
		}

		public async Task<IActionResult> Result(string id = "00000000-AAAA-AAAA-AAAA-AAAAAAAAA002") {
			try {
				//GetDistanceYear();

				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(o => o.Id == id);
				if (mangrove == null) {
					return NotFound($"Không tìm thấy cây có ID = {id}");
				}
				else {
					var photos = await context.TblPhotos.Where(o => o.IdObj == id).ToListAsync();
					var photoMangrove = await context.TblPhotos.Where(o => o.ImageNameId == mangrove.MainImage).FirstOrDefaultAsync();

					if (photoMangrove != null) {
						photos.Remove(photoMangrove);
						photos.Insert(0, photoMangrove);
					}

					TempData["Photos"] = photos;

					mangrove.View += 1;
					await context.SaveChangesAsync();

					return View(mangrove);
				}
			}
			catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
				return NotFound("Không kết nối được với Cơ sở dữ liệu");
			}
		}

		// Hàm riêng
		// Truy vấn thời gian (năm) chi filter tìm kiếm
		private async void GetDistanceYear() {
			try {
				var home = await context.TblHomes.FirstOrDefaultAsync();
				if (home == null) {
					Console.WriteLine("Bảng Home không có dữ liệu");
				}
				else {
					TempData["YearStart"] = home.YearStart;
					TempData["YearEnd"] = DateTime.Now.Year;
				}
			}
			catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
			}
		}
	}
}
