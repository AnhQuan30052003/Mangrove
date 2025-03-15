// Theo dõi cuộn màn hình 
function showOrHiddenIconFooter() {
	window.addEventListener("scroll", function () {
		try {
			// Nút cuộn lên đầu trang và nút tìm kiếm
			const btnScrollUp = document.querySelector("#scrollUp");
			const btnSearch = document.querySelector("#bottom_search");

			if (window.scrollY > window.innerHeight * 1 / 8) {
				btnScrollUp.classList.add("opacity")
				btnSearch.classList.add("opacity")
			} else {
				btnScrollUp.classList.remove("opacity")
				btnSearch.classList.remove("opacity")
			}
		}
		catch { }
	});
}
showOrHiddenIconFooter();