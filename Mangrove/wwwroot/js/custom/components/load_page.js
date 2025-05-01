// Theo dõi biểu tượng khi load page và xoá
function loadPage() {
	window.addEventListener("load", function () {
		removeIconLoadPage();
		highlightTabHeader();
		revoveryPositionPage();

		const pointerLastInputs = document.querySelectorAll(".pointer_last");
		if (pointerLastInputs.length > 0) {
			pointerLastInputs.forEach((item) => {
				item.dispatchEvent(new Event("focus"));
			});
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

				const time = 10;
				setTimeout(() => {
					loadingNotifier.classList.add("d-none");
				}, time * 1000);
			});
		});
	}
	catch { }
}
loading();

// Fix length focus seach input 
function movePointerInput() {
	const pointerLastInputs = document.querySelectorAll(".pointer_last");
	if (pointerLastInputs.length > 0) {
		pointerLastInputs.forEach((item) => {
			item.addEventListener("focus", function () {
				let value = item.value;
				item.value = "";
				item.value = value;
			});
		});
	}
}
movePointerInput();

// Cuộn tới vị trí sau khi reload
export function revoveryPositionPage() {
	try {
		let load = false;
		const url = location.href;
		const listPageRecovery = [
			"/Statistical/Page_Fillter",
			"/Statistical/Page_Overview"
		];
		listPageRecovery.forEach((page) => {
			if (url.includes(page)) {
				load = true;
				return;
			}
		});
		
		if (!load) return;

		const savedPosition = localStorage.getItem("scrollPosition");
		if (savedPosition !== null) {
			window.scrollTo({
				top: parseInt(savedPosition, 10),
				behavior: "instant"
			});
			localStorage.removeItem("scrollPosition");
		}
	}
	catch { }
}