// Theo dõi biểu tượng khi load page và xoá
export function loadPage() {
	window.addEventListener("load", function () {
		removeIconLoadPage();
		highlightTabHeader();

		const searchAJAX = document.querySelector(".search_ajax");
		if (searchAJAX) {
			searchAJAX.dispatchEvent(new Event("focus"));
		}
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
	try {
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
	catch { }
}

// Hiển thị loading trạng thái đang chờ
function loading() {
	try {
		const btnLoadings = document.querySelectorAll(".btn_loading");
		btnLoadings.forEach((btn) => {
			btn.addEventListener("click", function () {
				const loadingNotifier = document.querySelector("#loading_notifier");
				loadingNotifier.classList.remove("d-none");
			});
		});
	}
	catch { }
}
loading();

// Fix length focus seach input 
function movePointerInput() {
	const searchAJAX = document.querySelector(".search_ajax");
	if (searchAJAX) {
		searchAJAX.addEventListener("focus", function () {
			let value = searchAJAX.value;
			searchAJAX.value = "";
			searchAJAX.value = value;
		});
	}
}
movePointerInput();