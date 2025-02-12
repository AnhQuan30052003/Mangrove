// Theo dõi cuộn màn hình nút quay lên #scrollUp
document.addEventListener("scroll", function () {
	const button = document.querySelector("#scrollUp");
	if (window.scrollY > window.innerHeight * 1 / 8) {
		button.classList.add("opacity")
	} else {
		button.classList.remove("opacity")
	}
});

// Click icon search
const searchIcon = document.querySelector("#searchIcon");
searchIcon.addEventListener("click", function () {
	const searchFormUser = document.querySelector("#search_form_user");
	searchFormUser.classList.toggle("toggle_search_frame");

	const iconSearch = searchIcon.querySelector(".icon");
	if (iconSearch.classList.contains("no-click")) {
		iconSearch.classList.remove("no-click", "fa", "fa-search");
		iconSearch.classList.add("fa-solid", "fa-x");
	}
	else {
		iconSearch.classList.add("no-click", "fa", "fa-search");
		iconSearch.classList.remove("fa-solid", "fa-x");
	}
});

// Theo dõi chuyển tab của btn-option search-form home
document.querySelector(".search-form .options").addEventListener("click", function (e) {
	const itemClick = e.target;
	if (itemClick.matches(".btn-option")) {
		const div_options = document.querySelectorAll(".search-form .options .btn-option");
		const results = document.querySelectorAll(".search_frame .result_search");

		for (let i = 0; i < div_options.length; i++) {
			div_options[i].classList.remove("active")
			results[i].classList.add("d-none")
		}

		itemClick.classList.add("active");
		document.querySelector(itemClick.classList.contains("btn_search_keyword") ? ".search_keyword" : ".search_advance").classList.remove("d-none")
	}
});

// Theo dõi thay đổi ngôn ngữ
const dropdownItem = document.querySelectorAll(".language-dropdown .dropdown .dropdown-menu .dropdown-item");
dropdownItem.forEach((item) => {
	item.addEventListener("click", function (e) {
		const lang = this.getAttribute("data-value");
		const select = document.querySelector("#google_translate_element .goog-te-gadget .goog-te-combo");
		if (select) {
			select.value = lang;
			select.dispatchEvent(new Event("change"));
			console.log("Language changed: " + lang);
		}
	});
});

// Theo dõi thay đổi khi click đóng khung tìm kiếm
document.addEventListener("click", function (event) {
	const searchIcon = document.querySelector("#searchIcon");
	if (searchIcon.contains(event.target)) return;

	const sectionSearch = document.querySelector("#search_form_user");
	const searchForm = sectionSearch.querySelector(".search-form");

	// Khi SearchForm đang mở mà click ra ngoài 
	if (!searchForm.contains(event.target) && sectionSearch.classList.contains("toggle_search_frame")) {
		searchIcon.dispatchEvent(new Event("click"));
	}
});





//--
console.log("Run file site.js");