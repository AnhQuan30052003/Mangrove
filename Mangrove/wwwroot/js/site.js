// Theo dõi cuộn màn hình nút quay lên #scrollUp
document.addEventListener("scroll", function () {
	const button = document.querySelector("#scrollUp");
	if (window.scrollY > window.innerHeight * 1 / 8) {
		button.classList.add("opacity")
	} else {
		button.classList.remove("opacity")
	}
});
												  
// Theo dõi button language
const btnLanguage = document.querySelector(".btn_language");
if (btnLanguage) {
	btnLanguage.addEventListener("click", function (e) {
		const dropdown = btnLanguage.closest(".language").querySelector(".language_dropdown");
		dropdown.classList.toggle("d-none");
	});
}

// Click icon search
const searchIcon = document.querySelector("#searchIcon");
if (searchIcon) {
	searchIcon.addEventListener("click", function () {
		const searchFormUser = document.querySelector("#search_form_user");
		const background = document.querySelector("#background");
		searchFormUser.classList.toggle("d-none");
		background.classList.toggle("d-none");
	});
}

// Theo dõi chuyển tab của btn-option search-form home
document.querySelector("#search_form_user .options").addEventListener("click", function (e) {
	const itemClick = e.target;
	if (itemClick.matches(".btn-option")) {
		const div_options = this.querySelectorAll(".btn-option");
		const results = document.querySelectorAll("#search_form_user .result_search");

		for (let i = 0; i < div_options.length; i++) {
			div_options[i].classList.remove("active")
			results[i].classList.add("d-none")
		}

		itemClick.classList.add("active");
		document.querySelector(itemClick.classList.contains("btn_search_keyword") ? ".search_keyword" : ".search_advance").classList.remove("d-none")
	}
});

// Theo dõi thay đổi ngôn ngữ
const lis = document.querySelectorAll(".language_dropdown li");
lis.forEach((item) => {
	item.addEventListener("click", function (e) {
		const lang = this.getAttribute("data-value");
		const select = this.closest(".language").querySelector("#google_translate_element select");
		if (select) {
			select.value = lang;
			select.dispatchEvent(new Event("change"));
			console.log("Language changed: " + lang);
		}
	});
});

// Theo dõi huỷ khi click ngoài đối tượng
document.addEventListener("click", function (event) {
	// Nếu không phải click vào SearchIcon & SearchForm đang mở mà click ra ngoài
	const searchIcon = document.querySelector("#searchIcon");
	const sectionSearch = document.querySelector("#search_form_user");
	const background = document.querySelector("#background");
	if (!searchIcon.contains(event.target) && !sectionSearch.contains(event.target)) {
		sectionSearch.classList.add("d-none");
		background.classList.add("d-none");
	}

	// Nếu không click vào button language
	const btnLanguage = document.querySelector(".btn_language");
	const dropdown = btnLanguage.closest(".language").querySelector(".language_dropdown");
	if (!btnLanguage.contains(event.target) && !dropdown.classList.contains("d-none")) {
		dropdown.classList.add("d-none")
	}
});






//--
console.log("Run file site.js");