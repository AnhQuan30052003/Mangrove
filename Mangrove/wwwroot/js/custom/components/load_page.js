// Theo dõi biểu tượng khi load page và xoá
export function loadPage() {
	window.addEventListener("load", function () {
		removeIconLoadPage();
		highlightTabHeader();
	});
}
loadPage();

// Xoá icon khi load page xong
function removeIconLoadPage() {
	const preloader = document.querySelector(".preloader");
	if (preloader) preloader.remove();
}

// Làm nổi bật tab cần highlight
function highlightTabHeader() {
	const width = screen.width;
	if (width <= 1200) return;

	const pageOptions = document.querySelectorAll(".page_option");
	const typePage = document.querySelector("#typePage").getAttribute("type-page");

	pageOptions.forEach((item) => {
		const page = item.getAttribute("page");
		if (page == typePage) {
			item.classList.add("active_underline");
			return;
		}
		else item.classList.remove("active_underline");
	});
}
