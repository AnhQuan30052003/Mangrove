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
			GetDistanceYear();
			//TempData["Status"] = Helper.StatusNoifier.success;
			//TempData["Content"] = "Thành công test code";

			int quantityShow = 6;
			var mangroves = await context.TblMangroves
			.Skip(0)
			.Take(quantityShow)
			.ToListAsync();

			return View(mangroves);
		}

		public IActionResult Results() {
			GetDistanceYear();

			return View();
		}

		public IActionResult Result(string id) {
			GetDistanceYear();

			return View();
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
