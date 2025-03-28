using Mangrove.Data;
using Mangrove.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mangrove.Controllers {
	public class MangroveController : Controller {
		private readonly MangroveContext context;

		public MangroveController(MangroveContext context) {
			this.context = context;
		}

		// Danh sách 
		public async Task<IActionResult> Page_Index(string? search = null, int currentPage = 1, int? pageSize = null, string? sortType = null, string? sortFollow = null) {
			try {
				// Setup
				if (pageSize == null) pageSize = InfomationPaginate.GetFistPageSize();
				if (sortType == null) sortType = Helper.Key.sortASC;
				string findText = search ?? "";
				ViewData["Search"] = findText;

				bool isEN = Helper.Func.IsEnglish();

				var listTitleVI = new List<string> { "STT", "Tên", "Tên khác", "Tên khoa học", "Họ", "Phân bố", "Số cá thể", "Cập nhật lần cuối", "Tuỳ chọn" };
				var listTitleEN = new List<string> { "No", "Name", "Common name", "Scientific name", "Familia", "Distribution", "Number of individuals", "Last updated", "Options" };
				var listTitle = isEN ? listTitleEN : listTitleVI;

				int index = 1;
				var sortOptionsVI = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleVI[index++], item => item.NameVi },
					{ listTitleVI[index++], item => item.CommonNameVi },
					{ listTitleVI[index++], item => item.ScientificName },
					{ listTitleVI[index++], item => item.Familia },
					{ listTitleVI[index++], item => item.DistributionVi },
					{ listTitleVI[index++], item => item.TblIndividuals.Count() },
					{ listTitleVI[index++], item => item.UpdateLast },
				};

				index = 1;
				var sortOptionsEN = new Dictionary<string, Expression<Func<TblMangrove, object>>>()
				{
					{ listTitleEN[index++], item => item.NameEn },
					{ listTitleEN[index++], item => item.CommonNameEn },
					{ listTitleEN[index++], item => item.ScientificName },
					{ listTitleEN[index++], item => item.Familia },
					{ listTitleEN[index++], item => item.DistributionEn },
					{ listTitleEN[index++], item => item.TblIndividuals.Count() },
					{ listTitleEN[index++], item => item.UpdateLast },
				};
				var sortOptions = isEN ? sortOptionsEN : sortOptionsVI;

				// Tạo query
				var query = context.TblMangroves.Include(item => item.TblIndividuals).AsQueryable();

				// Kiểm tra nếu có thuộc tính cần sắp xếp
				if (!string.IsNullOrEmpty(sortFollow) && sortOptions.ContainsKey(sortFollow)) {
					var sortExpression = sortOptions[sortFollow];
					query = sortType == "asc" ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
				}

				var data = await query.ToListAsync();

				// Xử lý logic tìm kiếm
				List<TblMangrove> fillter = new List<TblMangrove>();
				foreach (var item in data) {
					var conditions = new List<string>();
					conditions.Add(item.NameVi);
					conditions.Add(item.NameEn);
					conditions.Add(item.CommonNameVi);
					conditions.Add(item.CommonNameEn);
					conditions.Add(item.ScientificName);
					conditions.Add(item.Familia);
					conditions.Add(item.DistributionVi);
					conditions.Add(item.DistributionEn);

					if (Helper.Func.CheckContain(findText, conditions)) fillter.Add(item);
				}
				var info = new InfomationPaginate(listTitle, currentPage, (int)pageSize, fillter.Count(), sortType, sortFollow, findText, "Mangrove", "Page_Index");
				var pagi = new Paginate_VM<TblMangrove>(fillter, info);

				return View(pagi);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Tạo mới
		public IActionResult Page_Create() {
			var model = new Mangrove_Admin_VM();
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Page_Create(Mangrove_Admin_VM model) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				// Begin validate
				Helper.Validate.Clear();
				foreach (var photo in model.Photos) {
					Helper.Validate.NotEmpty(photo.Image.DataBase64);
					Helper.Validate.NotEmpty(photo.NoteImgEn);
					Helper.Validate.NotEmpty(photo.NoteImgVi);
				}
				Helper.Validate.NotEmpty(model.NameEn);
				Helper.Validate.NotEmpty(model.NameVi);
				Helper.Validate.NotEmpty(model.ScientificName);
				Helper.Validate.NotEmpty(model.CommonNameEn);
				Helper.Validate.NotEmpty(model.CommonNameVi);
				Helper.Validate.NotEmpty(model.Familia);
				Helper.Validate.NotEmpty(model.MorphologyEn);
				Helper.Validate.NotEmpty(model.MorphologyVi);
				Helper.Validate.NotEmpty(model.EcologyEn);
				Helper.Validate.NotEmpty(model.EcologyVi);
				Helper.Validate.NotEmpty(model.DistributionEn);
				Helper.Validate.NotEmpty(model.DistributionVi);
				Helper.Validate.NotEmpty(model.ConservationStatusEn);
				Helper.Validate.NotEmpty(model.ConservationStatusVi);
				Helper.Validate.NotEmpty(model.UseEn);
				Helper.Validate.NotEmpty(model.UseVi);

				// Khi không có ảnh nào
				if (!model.Photos.Any()) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "Must have at least one photo !" : "Phải có ít nhất một ảnh !",
						Helper.SetupNotifier.Timer.midTime,
						""
					);
					return RedirectToAction("Page_Create");
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					return View(model);
				}
				// End validate

				// Lưu dữ liệu
				string idMangrve = Helper.Func.CreateId();
				string mainImage = "";
				bool saveMainImage = false;

				// Phần ảnh
				for (int i = 0; i < model.Photos.Count(); i++) {
					var photo = model.Photos[i];
					var image = photo.Image;
					string idImage = Helper.Func.CreateId();
					string fileName = $"{idImage}_{model.NameVi}.{image.DataType.Replace("image/", "").Replace("jpeg", "jpg")}";

					bool statusSave = await Helper.Func.SaveImageFromBase64Data(image.DataBase64, Helper.Path.treeImg, fileName);
					if (statusSave) {
						var photoDB = new TblPhoto {
							Id = Helper.Func.CreateId(),
							IdObj = idMangrve,
							ImageName = fileName,
							NoteImgEn = photo.NoteImgEn,
							NoteImgVi = photo.NoteImgVi,
						};
						context.TblPhotos.Add(photoDB);

						if (!saveMainImage) {
							saveMainImage = true;
							mainImage = fileName;
						}

					}
				}

				// Lưu cây
				var mangrove = new TblMangrove {
					Id = idMangrve,
					NameEn = model.NameEn,
					NameVi = model.NameVi,
					CommonNameEn = model.CommonNameEn,
					CommonNameVi = model.CommonNameVi,
					ScientificName = model.ScientificName,
					Familia = model.Familia,
					MainImage = mainImage,
					MorphologyEn = model.MorphologyEn,
					MorphologyVi = model.MorphologyVi,
					EcologyEn = model.EcologyEn,
					EcologyVi = model.EcologyVi,
					DistributionEn = model.DistributionEn,
					DistributionVi = model.DistributionVi,
					ConservationStatusEn = model.ConservationStatusEn,
					ConservationStatusVi = model.ConservationStatusVi,
					UseEn = model.UseEn,
					UseVi = model.UseVi,
					View = 0,
					UpdateLast = DateTime.Now
				};
				context.TblMangroves.Add(mangrove);

				await context.SaveChangesAsync();

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Create successfully." : "Tạo thành công.",
					Helper.SetupNotifier.Timer.fastTime,
					""
				);

				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Chỉnh sửa
		public async Task<IActionResult> Page_Edit(string id) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var model = await context.TblMangroves
				.Select(item => new Mangrove_Admin_VM {
					Id = item.Id,
					MainImage = item.MainImage,
					NameEn = item.NameEn,
					NameVi = item.NameVi,
					CommonNameEn = item.CommonNameEn,
					CommonNameVi = item.CommonNameVi,
					ScientificName = item.ScientificName,
					Familia = item.Familia,
					MorphologyEn = item.MorphologyEn,
					MorphologyVi = item.MorphologyVi,
					EcologyEn = item.EcologyEn,
					EcologyVi = item.EcologyVi,
					DistributionEn = item.DistributionEn,
					DistributionVi = item.DistributionVi,
					UseEn = item.UseEn,
					UseVi = item.UseVi
				})
				.FirstOrDefaultAsync(item => item.Id == id);

				if (model == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "The edit page you just visited does not exist !" : "Trang chỉnh sửa vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.midTime
					);
					return RedirectToAction("Page_Index");
				}

				// Danh sách hình ảnh
				List<Photo_Mangrove_Admin_VM> listPhoto = await context.TblPhotos
				.Where(item => item.IdObj == id)
				.Select(item => new Photo_Mangrove_Admin_VM {
					Id = item.Id,
					ImageName = item.ImageName,
					NoteImgEn = item.NoteImgEn ?? "",
					NoteImgVi = item.NoteImgVi ?? "",
					Image = new DataImage {
						DataType = "type",
						DataBase64 = "base64"
					}
				})
				.ToListAsync();

				// Xử lý thứ tự ảnh banner slick slider
				if (listPhoto.Count() > 1) {
					foreach (var photo in listPhoto) {
						if (photo.ImageName == model.MainImage) {
							var save = photo;
							listPhoto.Remove(save);
							listPhoto.Insert(0, save);
							break;
						}
					}
				}

				model.Photos = listPhoto;

				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}
		[HttpPost]
		public async Task<IActionResult> Page_Edit(Mangrove_Admin_VM model) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				// Begin validate
				Helper.Validate.Clear();
				ViewData["NotPhoto"] = string.Empty;

				foreach (var photo in model.Photos) {
					Helper.Validate.NotEmpty(photo.Image.DataBase64);
					Helper.Validate.NotEmpty(photo.NoteImgEn);
					Helper.Validate.NotEmpty(photo.NoteImgVi);
				}
				Helper.Validate.NotEmpty(model.NameEn);
				Helper.Validate.NotEmpty(model.NameVi);
				Helper.Validate.NotEmpty(model.ScientificName);
				Helper.Validate.NotEmpty(model.CommonNameEn);
				Helper.Validate.NotEmpty(model.CommonNameVi);
				Helper.Validate.NotEmpty(model.Familia);
				Helper.Validate.NotEmpty(model.MorphologyEn);
				Helper.Validate.NotEmpty(model.MorphologyVi);
				Helper.Validate.NotEmpty(model.EcologyEn);
				Helper.Validate.NotEmpty(model.EcologyVi);
				Helper.Validate.NotEmpty(model.DistributionEn);
				Helper.Validate.NotEmpty(model.DistributionVi);
				Helper.Validate.NotEmpty(model.ConservationStatusEn);
				Helper.Validate.NotEmpty(model.ConservationStatusVi);
				Helper.Validate.NotEmpty(model.UseEn);
				Helper.Validate.NotEmpty(model.UseVi);

				// Khi không có ảnh nào
				if (!model.Photos.Any()) {
					ViewData["NotPhoto"] = isEN ? "Must have at least one photo !" : "Phải có ít nhất một ảnh !";
					return View(model);
				}

				// Trả lại view nếu có lỗi validate
				if (Helper.Validate.HaveError()) {
					return View(model);
				}
				// End validate

				// Xử lý hình ảnh và dữ liệu
				// Nếu có ảnh mới
				foreach (var item in model.Photos) {
					string fileName = $"{item.Id}_{item.NoteImgVi}.{item.Image.DataType.Replace("image/", "").Replace("jpeg", "jpg")}";
					if (item.Image.DataBase64 != "base64") {
						bool statusSave = await Helper.Func.SaveImageFromBase64Data(
							item.Image.DataBase64,
							Helper.Path.treeImg,
							fileName
						);

						if (statusSave && item.NoteImgVi != fileName) {
							Helper.Func.DeletePhoto(Helper.Path.treeImg, item.NoteImgVi);
							item.ImageName = fileName;
						}
					}
					else {
						if (item.ImageName != fileName) {
							string extension = Path.GetExtension(item.ImageName);
							fileName = $"{item.Id}_{item.NoteImgVi}{extension}";
							string oldPath = Path.Combine(Helper.Path.treeImg, item.ImageName);
							string newPath = Path.Combine(Helper.Path.treeImg, fileName);

							if (System.IO.File.Exists(oldPath)) {
								System.IO.File.Move(oldPath, newPath);
								item.ImageName = fileName;
							}
						}
					}

					var photo = await context.TblPhotos.FirstOrDefaultAsync(p => p.Id == item.Id);
					if (photo != null) {
						photo.NoteImgEn = item.NoteImgEn;
						photo.NoteImgVi = item.NoteImgVi;
						photo.ImageName = item.ImageName;
						context.TblPhotos.Update(photo);
					}
				}

				// Cây
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == model.Id);
				if (mangrove == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? $"There was an error editing the mangrove !" : $"Có lỗi trong quá trình chỉnh sửa cây ngập mặn !",
						Helper.SetupNotifier.Timer.midTime
					);
					return RedirectToAction("Page_Index");
				}

				// Gõ code cập nhật cho cây !
				mangrove.MainImage = model.MainImage;
				mangrove.NameEn = model.NameEn;
				mangrove.NameVi = model.NameVi;
				mangrove.CommonNameEn = model.CommonNameEn;
				mangrove.CommonNameVi = model.CommonNameVi;
				mangrove.ScientificName = model.ScientificName;
				mangrove.Familia = model.Familia;
				mangrove.MorphologyEn = model.MorphologyEn;
				mangrove.MorphologyVi = model.MorphologyVi;
				mangrove.EcologyEn = model.EcologyEn;
				mangrove.EcologyVi = model.EcologyVi;
				mangrove.DistributionEn = model.DistributionEn;
				mangrove.DistributionVi = model.DistributionVi;
				mangrove.ConservationStatusEn = model.ConservationStatusEn;
				mangrove.ConservationStatusVi = model.ConservationStatusVi;
				mangrove.UseEn = model.UseEn;
				mangrove.UseVi = model.UseVi;
				mangrove.UpdateLast = DateTime.Now;

				context.TblMangroves.Update(mangrove);
				await context.SaveChangesAsync();

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Edit successfully." : "Chỉnh sửa thành công.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Chi tiết
		public async Task<IActionResult> Page_Detail(string id) {
			try {
				bool isEN = Helper.Func.IsEnglish();

				var model = await context.TblMangroves
				.Select(item => new Mangrove_Client_VM {
					Id = item.Id,
					MainImage = item.MainImage,
					Name = isEN ? item.NameEn : item.NameVi,
					CommonName = isEN ? item.CommonNameEn : item.CommonNameVi,
					ScientificName = item.ScientificName,
					Familia = item.Familia,
					Use = isEN ? item.UseEn : item.UseVi,
					Morphology = isEN ? item.MorphologyEn : item.MorphologyVi,
					Ecology = isEN ? item.EcologyEn : item.EcologyVi,
					Distribution = isEN ? item.DistributionEn : item.DistributionVi,
					ConservationStatus = isEN ? item.ConservationStatusEn : item.ConservationStatusVi,
					View = item.View,
				})
				.FirstOrDefaultAsync(item => item.Id == id);

				if (model == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "The detail page you just visited does not exist !" : "Trang chi tiết vừa truy cập không tồn tại !",
						Helper.SetupNotifier.Timer.midTime
					);
					return RedirectToAction("Page_Index");
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
						if (photo.Image == model.MainImage) {
							var save = photo;
							listPhoto.Remove(save);
							listPhoto.Insert(0, save);
							break;
						}
					}
				}

				model.Photos = listPhoto;

				return View(model);
			}
			catch (Exception ex) {
				string notifier = $"-----\nCó lỗi khi kết nối với Cơ sở dữ liệu.\n-----\nError: {ex.Message}";
				Console.WriteLine(notifier);
				return NotFound(notifier);
			}
		}

		// Xoá
		public async Task<IActionResult> Page_Delete(string id) {
			bool isEN = Helper.Func.IsEnglish();
			try {
				// Xoá đối tượng
				var mangrove = await context.TblMangroves.FirstOrDefaultAsync(item => item.Id == id);
				if (mangrove == null) {
					Helper.Notifier.Create(
						Helper.SetupNotifier.Status.fail,
						isEN ? "Mangrove to delete not found !" : "Không tìm thấy cây ngập mặn cần xoá !",
						Helper.SetupNotifier.Timer.midTime,
						""
					);
					return RedirectToAction("Page_Index");
				}

				// Xoá ảnh
				var mangrovePhotos = await context.TblPhotos.Where(item => item.IdObj == id).ToListAsync();
				if (mangrovePhotos.Any()) {
					foreach (var photo in mangrovePhotos) {
						context.TblPhotos.Remove(photo);
						Helper.Func.DeletePhoto(Helper.Path.treeImg, photo.ImageName);
					}
				}

				context.TblMangroves.Remove(mangrove);
				await context.SaveChangesAsync();

				// Setup thông báo
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.success,
					isEN ? "Delete successfully." : "Đã xoá thành công.",
					Helper.SetupNotifier.Timer.shortTime,
					""
				);

				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
			catch {
				Helper.Notifier.Create(
					Helper.SetupNotifier.Status.fail,
					isEN ? "Mangrove delete failed !" : "Xoá cây ngập mặn thất bại !",
					Helper.SetupNotifier.Timer.midTime,
					""
				);
				return Content(Helper.Link.GetUrlBack(), "text/html");
			}
		}
	}
}
