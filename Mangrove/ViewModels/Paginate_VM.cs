using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mangrove.ViewModels {
	public class Paginate_VM<T> {
		public InfomationPaginate InfomationPaginate;
		public List<T> DataPaginate;

		public Paginate_VM(List<T> DataPaginate, InfomationPaginate InfomationPaginate) {
			this.InfomationPaginate = InfomationPaginate;
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
		private static readonly List<int> ListPageSize = new List<int> { 5, 10, 20, 50, 100, 200, 500, 1000 };
		public SelectList SelectListPageSize;
		public List<string> ListTitle;

		public int CurrentPage;
		public int PageSize;
		public int TotalPages;
		public string sortType;
		public string? sortFollow;

		public string FindText;
		public string Controller;
		public string Action;

		public InfomationPaginate(List<string> ListTitle, int CurrentPage, int PageSize, int totalItem, string sortType, string? sortFollow, string FindText, string Controller, string Action) {
			string label = Helper.Func.IsEnglish() ? " line" : " dòng";

			SelectListPageSize = new SelectList(
				ListPageSize.Select(item => new {
					Value = item,
					Text = item + label
				}),
				"Value",
				"Text",
				PageSize
			);
			this.ListTitle = ListTitle ?? new List<string>();

			this.CurrentPage = CurrentPage;
			this.PageSize = PageSize;
			TotalPages = (int) Math.Ceiling((double) totalItem / PageSize);
			this.sortType = sortType;
			this.sortFollow = sortFollow;

			this.FindText = FindText;
			this.Controller = Controller;
			this.Action = Action;
		}
	
		public static int GetFistPageSize() => ListPageSize[0];	
	}
}
