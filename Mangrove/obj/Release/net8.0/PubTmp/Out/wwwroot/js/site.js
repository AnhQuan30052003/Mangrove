// Theo dõi khi tải xong trang web
function listenrLoadDone() {
	window.addEventListener("load", function (e) {
		// Theo dõi biểu tượng khi load page và xoá
		const preloader = document.querySelector(".preloader");
		if (preloader) preloader.remove();
	});
}
listenrLoadDone();

// Theo dõi click hamburger menu
function listnerClickHamburgerMenu() {
	try {
		const btnMenu = document.querySelector(".btn_menu");
		const iconS = btnMenu.querySelector(".icon-s");
		const iconX = btnMenu.querySelector(".icon-x");
		const listMenu = document.querySelector(".list_menu");

		iconS.classList.toggle("d-none");
		iconX.classList.toggle("d-none");
		listMenu.classList.toggle("right-0");

		if (listMenu.classList.contains("right-0")) {
			document.body.style.overflow = "hidden";
		}
		else {
			document.body.style.overflow = "auto";
		}
	}
	catch {
		console.log("Có lỗi khi click vào Hamburger Menu");
	}
}

// Theo dõi cuộn màn hình 
function listenerScrollPage() {
	window.addEventListener("scroll", function () {
		// Nút cuộn lên đầu trang và nút tìm kiếm
		const btnScrollUp = document.querySelector("#scrollUp");
		const btnSearch = document.querySelector("#bottom_search");
		if (!btnScrollUp || !btnSearch) return;

		if (window.scrollY > window.innerHeight * 1 / 8) {
			btnScrollUp.classList.add("opacity")
			btnSearch.classList.add("opacity")
		} else {
			btnScrollUp.classList.remove("opacity")
			btnSearch.classList.remove("opacity")
		}
	});
}
listenerScrollPage();

// Click icon search
// const searchIcon = document.querySelector("#searchIcon");
// if (searchIcon) {
// 	searchIcon.addEventListener("click", function () {
// 		const searchFormUser = document.querySelector("#search_form_user");
// 		const background = document.querySelector("#background");
// 		const iconX = searchIcon.querySelector(".icon-x");
// 		const iconS = searchIcon.querySelector(".icon-s");

// 		searchFormUser.classList.toggle("d-none");
// 		background.classList.toggle("d-none");
// 		iconX.classList.toggle("d-none");
// 		iconS.classList.toggle("d-none");
// 	});
// }

// Theo dõi chuyển tab của btn-option search-form home
// document.querySelector("#search_form_user .options").addEventListener("click", function (e) {
// 	const itemClick = e.target;
// 	if (itemClick.matches(".btn-option")) {
// 		const div_options = this.querySelectorAll(".btn-option");
// 		const results = document.querySelectorAll("#search_form_user .result_search");

// 		for (let i = 0; i < div_options.length; i++) {
// 			div_options[i].classList.remove("active")
// 			results[i].classList.add("d-none")
// 		}

// 		itemClick.classList.add("active");
// 		document.querySelector(itemClick.classList.contains("btn_search_keyword") ? ".search_keyword" : ".search_advance").classList.remove("d-none")
// 	}
// });

// Tiến hành thay đổi ngôn ngữ
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
function listenerChangeLanguage() {
	// Khi click vào button language
	const btnLanguage = document.querySelector(".btn_language");
	if (btnLanguage) {
		btnLanguage.addEventListener("click", function (e) {
			const dropdown = btnLanguage.closest(".language").querySelector(".language_dropdown");
			const iconDrop = btnLanguage.querySelector("span i");
			if (!dropdown || !iconDrop) return;

			dropdown.classList.toggle("d-none");
			iconDrop.classList.toggle("_180deg");
		});
	}

	// Khi click từng icon thì thay đổi ngôn ngữ
	const lis = document.querySelectorAll(".language_dropdown li");
	lis.forEach((item) => {
		item.addEventListener("click", function (e) {
			const lang = this.getAttribute("data-value");
			changeLanguage(lang);
		});
	});
}
listenerChangeLanguage();

// Setup ngày
function setupDay() {
	try {
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
	catch {
		console.log("Có lỗi khit cập nhật setupDay");
	}
}
// setupDay();

// Toggle info title
function toggleInfoTitle() {
	const toggleTitles = document.querySelectorAll(".toggle_title");
	toggleTitles.forEach((item) => {
		item.addEventListener("click", function () {
			const info = item.closest(".box").querySelector(".info");
			if (info) info.classList.toggle("box_collapse");
		});
	});
}
toggleInfoTitle();

// Theo dõi mở rộng/thu gọn nội dung trong result
function listenerToggleExpandOrCollapseContentResult() {
	const selects = document.querySelectorAll(".select_ex_co");
	selects.forEach((item) => {
		item.addEventListener("change", function () {
			// Đồng bộ 2 select mở rộng/thu gọn
			selects.forEach((select) => {
				if (select !== item) {
					select.value = item.value;
				}
			});

			// Xử lý sự kiện
			const infos = document.querySelectorAll(".box .info")
			if (item.value == "ex") {
				infos.forEach((info) => {
					info.classList.remove("box_collapse");
				});
			}
			else {
				infos.forEach((info) => {
					info.classList.add("box_collapse");
				});
			}
		});
	});
}
listenerToggleExpandOrCollapseContentResult();

// Theo dõi các slick slider
function loadSlider() {
	// Lấy - khởi tạo tham chiếu
	const slickSliders = document.querySelectorAll(".slick_slider");
	slickSliders.forEach((slickSlider) => {
		const slides = slickSlider.querySelector(".slides");
		let slideItem = slides.querySelectorAll(".slide_item");
		const description = document.querySelector(".show_description_photo p");

		// Nếu không có hoặc chỉ 1 slide_item thì không cần tạo Slick Slider
		if (slideItem.length <= 1) return;
		let loadSuccess = false;

		// Tạo node đầu và cuối slide
		const firstSlide = slideItem[0].cloneNode(true);
		const lastSlide = slideItem[slideItem.length - 1].cloneNode(true);
		slides.insertBefore(lastSlide, slideItem[0]);
		slides.appendChild(firstSlide);
		slideItem = slides.querySelectorAll(".slide_item");

		// Tạo 2 nút prev và next
		const btnPrev = document.createElement("button");
		const btnNext = document.createElement("button");
		btnPrev.classList.add("btn_prev");
		btnNext.classList.add("btn_next");
		btnPrev.innerHTML = "&#10094;"
		btnNext.innerHTML = "&#10095;"
		slickSlider.appendChild(btnPrev);
		slickSlider.appendChild(btnNext);

		// Xử lý khi click
		let index = 1;

		function updateSlide(loadSlide = true) {
			if (loadSlide) slides.style.transition = "all 0.5s ease-in-out";
			else slides.style.transition = "none";

			slides.style.transform = `translateX(-${index * 100}%)`;

			if (index == 0 || index == slideItem.length - 1) {
				index = (index == 0 ? slideItem.length - 2 : 1);
				setTimeout(() => updateSlide(false), 500);
			}

			const note = slideItem[index].getAttribute("note");
			description.innerHTML = note;
		}

		function prevSlide() {
			index -= 1;
			updateSlide();
		}

		function nextSlide() {
			index += 1;
			updateSlide();
		}

		if (!loadSuccess) {
			updateSlide(false);
			loadSuccess = true;
		}

		btnPrev.addEventListener("click", function () {
			prevSlide();
			clearInterval(autoPlaySlide);
			autoPlaySlide = setInterval(() => nextSlide(), 5000);
		});
		btnNext.addEventListener("click", function () {
			nextSlide();
			clearInterval(autoPlaySlide);
			autoPlaySlide = setInterval(() => nextSlide(), 5000);
		});

		let autoPlaySlide = setInterval(() => nextSlide(), 5000);
	});
}
loadSlider();

// Xử lý khi click vào ảnh phần thông tin cây
function listenerImageToShow() {
	const imageClick = document.querySelectorAll(".click_show_image");
	imageClick.forEach((item) => {
		item.addEventListener("click", function (e) {
			try {
				const src = item.getAttribute("src");
				const showImage = document.querySelector("#click_show_image");
				const img = showImage.querySelector(".box_show img");
				img.src = src;
				showImage.classList.remove("d-none");
				document.body.style.overflow = "hidden";
			}
			catch {
				console.log("Lỗi: Ảnh vừa click không thể phóng to !");
			}
		});
	});
}
listenerImageToShow();

// Theo dõi việc toggle QR img
function listenerClickButtonQR() {
	const btnQR = document.querySelectorAll(".toggleQR");
	btnQR.forEach((item) => {
		item.addEventListener("click", function () {
			const frameQR = item.closest(".individual_item").querySelector(".frame_qr");
			if (frameQR) frameQR.classList.toggle("height-222");
		});
	});
}
listenerClickButtonQR();





// Theo dõi huỷ khi click ngoài đối tượng
function listenerClickToClose() {
	document.addEventListener("click", function (event) {
		// Nếu không phải click vào SearchIcon & SearchForm đang mở mà click ra ngoài
		try {
			// const searchIcon = document.querySelector("#searchIcon");
			// const sectionSearch = document.querySelector("#search_form_user");
			// const background = document.querySelector("#background");
			// const iconX = searchIcon.querySelector(".icon-x");
			// const iconS = searchIcon.querySelector(".icon-s");
			// if (!searchIcon.contains(event.target) && !sectionSearch.contains(event.target) && iconS.classList.contains("d-none")) {
			// 	sectionSearch.classList.add("d-none");
			// 	background.classList.add("d-none");
			// 	iconX.classList.add("d-none");
			// 	iconS.classList.remove("d-none");
			// }
		}
		catch {
			console.log("Có lỗi khi tắt tìm kiếm");
		}

		// Nếu không click vào button language
		try {
			const btnLanguage = document.querySelector(".btn_language");
			const iconDrop = btnLanguage.querySelector("span i");
			const dropdown = btnLanguage.closest(".language").querySelector(".language_dropdown");
			if (!btnLanguage.contains(event.target) && !dropdown.classList.contains("d-none")) {
				dropdown.classList.add("d-none")
				iconDrop.classList.remove("_180deg");
			}
		}
		catch {
			console.log("Có lỗi khi tắt ngôn ngữ");
		}

		// Huỷ khi mở phóng to ảnh
		try {
			const showImg = document.querySelector("#click_show_image");
			const boxShow = showImg.querySelector(".box_show");
			if (!showImg.classList.contains("d-none") && showImg.contains(event.target) && !boxShow.contains(event.target)) {
				showImg.classList.add("d-none");
				document.body.style.overflow = "auto";
			}
		}
		catch {
			console.log("Có lỗi khi tắt show image");
		}
	});
}
listenerClickToClose();

//--
console.log("Run file site.js");