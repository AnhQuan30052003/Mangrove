using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Policy;

namespace Mangrove.Models {
	public class PaginateModel<T> {
		public List<T> DataPaginate;
		public InfomationPaginate InfomationPaginate;

		public PaginateModel(int CurrentPage, int PageSize, List<T> DataPaginate, string NameTable, List<string> ListTitle, string FindText, string Controller, string Action) {
			this.InfomationPaginate = new InfomationPaginate(
				NameTable,
				ListTitle,
				CurrentPage,
				PageSize,
				FindText,
				Controller,
				Action,
				DataPaginate.Count()
			);
			this.DataPaginate = ExcePaginate(DataPaginate);
		}
		private List<T> ExcePaginate(List<T> DataPaginate) {
			return DataPaginate
			.Skip((InfomationPaginate.CurrentPage - 1) * InfomationPaginate.PageSize)
			.Take(InfomationPaginate.PageSize)
			.ToList();
		}
	}
	public class InfomationPaginate {
		public static readonly List<int> ListPageSize = new List<int> { 5, 10, 20, 50, 100, 200, 500, 1000 };
		public SelectList SelectListPageSize;
		public List<string> ListTitle;
		public string NameTable;

		public int CurrentPage;
		public int PageSize;
		public string FindText;
		public string Controller;
		public string Action;
		public int TotalPages;

		public InfomationPaginate(string NameTable, List<string> ListTitle, int CurrentPage, int PageSize, string FindText, string Controller, string Action, int totalPages) {
			bool isEN = Helper.Func.IsLanguage("en");
			string label = Helper.Func.IsLanguage("en") ? " line" : " dòng";

			this.SelectListPageSize = new SelectList(
				ListPageSize.Select(item => new {
					Value = item,
					Text = item + label
				}),
				"Value",
				"Text",
				PageSize
			);
			this.NameTable = NameTable;
			this.ListTitle = ListTitle ?? new List<string>();
			
			this.CurrentPage = CurrentPage;
			this.PageSize = PageSize;
			this.FindText = FindText;
			this.Controller = Controller;
			this.Action = Action;
			this.TotalPages = (int)Math.Ceiling((double)totalPages / PageSize);
		}
	}
}
