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
		const iconDrop = btnLanguage.querySelector("span i");
		dropdown.classList.toggle("d-none");
		iconDrop.classList.toggle("_180deg");
	});
}

// Click icon search
const searchIcon = document.querySelector("#searchIcon");
if (searchIcon) {
	searchIcon.addEventListener("click", function () {
		const searchFormUser = document.querySelector("#search_form_user");
		const background = document.querySelector("#background");
		const iconX = searchIcon.querySelector(".icon-x");
		const iconS = searchIcon.querySelector(".icon-s");
		searchFormUser.classList.toggle("d-none");
		background.classList.toggle("d-none");
		iconX.classList.toggle("d-none");
		iconS.classList.toggle("d-none");
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

// Thay đổi ngôn ngữ
function changeLanguage(lang) {
	const select = document.querySelector("#google_translate_element select");
	if (select) {
		select.value = lang;
		select.dispatchEvent(new Event("change"));
		console.log("Language changed: " + lang);
		localStorage.setItem("language", lang);
	}
}

// Theo dõi thay đổi ngôn ngữ
const lis = document.querySelectorAll(".language_dropdown li");
lis.forEach((item) => {
	item.addEventListener("click", function (e) {
		const lang = this.getAttribute("data-value");
		changeLanguage(lang);
	});
});

// Theo dõi huỷ khi click ngoài đối tượng
document.addEventListener("click", function (event) {
	// Nếu không phải click vào SearchIcon & SearchForm đang mở mà click ra ngoài
	const searchIcon = document.querySelector("#searchIcon");
	const sectionSearch = document.querySelector("#search_form_user");
	const background = document.querySelector("#background");
	const iconX = searchIcon.querySelector(".icon-x");
	const iconS = searchIcon.querySelector(".icon-s");
	if (!searchIcon.contains(event.target) && !sectionSearch.contains(event.target) && iconS.classList.contains("d-none")) {
		sectionSearch.classList.add("d-none");
		background.classList.add("d-none");
		iconX.classList.add("d-none");
		iconS.classList.remove("d-none");
	}

	// Nếu không click vào button language
	const btnLanguage = document.querySelector(".btn_language");
	const iconDrop = btnLanguage.querySelector("span i");
	const dropdown = btnLanguage.closest(".language").querySelector(".language_dropdown");
	if (!btnLanguage.contains(event.target) && !dropdown.classList.contains("d-none")) {
		dropdown.classList.add("d-none")
		iconDrop.classList.remove("_180deg");
	}
});

// Setup ngày
function setupDay() {
	const years = document.querySelector("._years");
	const months = document.querySelector("._months");
	let countDays = 0;

	if (years.value == "" || months.value == "") {
		countDays = 31;
	}
	else {
		const getQuantityDay = new Date(years.value, months.value, 0);
		countDays = getQuantityDay.getDate();
	}

	// Xoá sách cần phần tử bên trong
	const selectDefault = document.querySelector(".select_default");
	const days = document.querySelector("._days");
	days.innerHTML = "";
	days.appendChild(selectDefault);

	for (let i = countDays; i > 0; i--) {
		const option = document.createElement("option");
		option.value = i;
		option.innerText = i;
		days.appendChild(option);
	}
}
setupDay();





//--
console.log("Run file site.js");