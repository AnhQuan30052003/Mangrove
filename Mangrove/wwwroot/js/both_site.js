// Theo dõi khi tải xong trang web
function listenrLoadDone() {
	// Theo dõi biểu tượng khi load page và xoá
	window.addEventListener("load", function () {
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
	catch { }
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

// Theo dõ click dropdown_menu tự động
function listnerToggleDropdownMenu() {
	const dropdownToggles = document.querySelectorAll(".dropdown_toggle");
	dropdownToggles.forEach((item) => {
		item.addEventListener("click", function (e) {
			const dropdownList = item.closest(".dropdown_menu").querySelector(".dropdown_list");
			dropdownList.classList.toggle("d-none");

			// Nếu nút đo là chuyển đổi ngôn ngữ
			if (item.classList.contains("btn_language")) {
				const iconDrop = item.querySelector("span i");
				iconDrop.classList.toggle("_180deg");
			}
		});
	});
}
listnerToggleDropdownMenu();

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

// Xử lý khi click vào ảnh phần thông tin cây và hiển thị nó to hơn
function listenerImageToShow() {
	const body = document.body;
	body.addEventListener("click", function (event) {
		const clicked = event.target;
		let scale;

		if (clicked.matches(".click_show_image")) {
			try {
				const src = clicked.getAttribute("src");
				const showImage = document.querySelector("#click_show_image");
				const img = showImage.querySelector(".box_show_img img");
				img.src = src;
				showImage.classList.remove("d-none");
				document.body.style.overflow = "hidden";
				scale = 1;
				img.style.transform = `scale(${scale})`;

				// Zoom ảnh khi cuộn chuột
				showImage.addEventListener("wheel", function (e) {
					if (e.deltaY < 0) {
						scale += 0.1;
						if (scale >= 3) scale = 3;
					} else {
						scale -= 0.1;
						if (scale < 0.5) scale = 0.5;
					}
					img.style.transform = `scale(${scale})`;
				});
			}
			catch { }
		}
	});
}
listenerImageToShow();

// Theo dõi việc toggle QR img
function listenerClickButtonQR() {
	const listIndividual = document.querySelector(".list_individuals");
	if (!listIndividual) return;

	listIndividual.addEventListener("click", function (event) {
		const clicked = event.target;
		if (clicked.matches(".toggleQR")) {
			const frameQR = clicked.closest(".individual_item").querySelector(".frame_qr");
			if (frameQR) frameQR.classList.toggle("height-222");
		}
	});
}
listenerClickButtonQR();

// Request with AJAX
function requestAjax(url, result = null) {
	try {
		const xhr = new XMLHttpRequest();
		xhr.open("get", url);
		xhr.setRequestHeader("REQUESTED", "AJAX");
		xhr.onload = function () {
			if (xhr.status == 200) {
				if (result != null) result.innerHTML = xhr.responseText;
			}
		}
		xhr.send();
	}
	catch {
		console.log("Lỗi request ajax tới: " + url);
	}
}

// Theo dõi việc tìm kiếm các cá thể trong một cây
function listenerSearchInvidiual() {
	const searchInvidiual = document.querySelector("#searchInvidiual");
	if (!searchInvidiual) return;

	let timer;
	searchInvidiual.addEventListener("input", function (e) {
		clearTimeout(timer);
		timer = setTimeout(() => {
			const value = this.value;
			const id = this.getAttribute("id-mangrove");

			const url = `/Home/Page_Result?id=${id}&searchIndividual=${value}`;
			const result = document.querySelector(".list_individuals");
			requestAjax(url, result);
		}, 300);
	});
}
listenerSearchInvidiual();

// Theo dõi việc tìm kiếm các cây trong thành phân loài
function listenerSearchMangrove() {
	const search = document.querySelector("#search_mangrove");
	if (!search) return;

	let timer;
	search.addEventListener("input", function (e) {
		clearTimeout(timer);
		timer = setTimeout(() => {
			const value = this.value;

			const url = `/Home/Page_SpeciesComposition?search=${value}`;
			const result = document.querySelector(".list_mangrove");
			requestAjax(url, result);
		}, 300);
	});
}
listenerSearchMangrove();

// Theo dõi khi click và tab giai đoạn của mỗi cây
function listenerClickPreviodOfTree() {
	try {
		const individualDisplay = document.querySelector(".individual_display");
		const tabs = individualDisplay.querySelectorAll(".tab")
		const displayItems = individualDisplay.querySelectorAll(".display_item");

		tabs.forEach((tab) => {
			tab.addEventListener("click", function (e) {
				tabs.forEach((item) => item.classList.remove("active"));
				this.classList.add("active");

				const idTab = this.getAttribute("tab");
				displayItems.forEach((item) => item.classList.add("d-none"));
				displayItems[idTab].classList.remove("d-none");
			});
		})
	}
	catch { }
}
listenerClickPreviodOfTree();

// Theo dõi xem ở page nào để hiển thị tab active khi không ở màn hình mobile
function listenerPageType() {
	try {
		const width = screen.width;
		if (width < 992) return;

		const url = location.href;
		const pageOptions = document.querySelectorAll(".page_option");
		pageOptions.forEach((item) => {
			const page = item.getAttribute("page");
			if (url.includes(page)) item.classList.add("active_underline");
			else item.classList.remove("active_underline");
		});

		const active = document.querySelectorAll(".active_underline");
		if (active.length == 0) pageOptions[0].classList.add("active_underline");
	}
	catch { }
}
listenerPageType();

// Theo dõi click hiện thông tin QR
function listenerClickShowQR() {
	try {
		const qrImgs = document.querySelectorAll(".click_show_qr");
		qrImgs.forEach((item) => {
			item.addEventListener("click", function () {
				const infoQR = document.querySelector(".info_qr");
				const qrDay = infoQR.querySelector(".qr_day");
				const qrPos = infoQR.querySelector(".qr_pos");
				const qrName = document.querySelector(".qr_name_mangrove");

				const showQRCode = document.querySelector("#show_qr_code");
				showQRCode.querySelector(".qr_name").innerHTML = qrName.textContent;
				showQRCode.querySelector(".qr_day").innerHTML = qrDay.textContent;
				showQRCode.querySelector(".qr_pos").innerHTML = qrPos.textContent;
				showQRCode.querySelector(".qr_img").src = item.getAttribute("src");
				showQRCode.classList.remove("d-none");
			});
		});
	}
	catch { }
}
listenerClickShowQR();


// Theo dõi huỷ khi click ngoài đối tượng
function listenerClickToClose() {
	// Event Click
	document.addEventListener("click", function (event) {
		// Nếu không click vào button language
		try {
			const dropdownToggles = document.querySelectorAll(".dropdown_toggle");
			dropdownToggles.forEach((item) => {
				const dropdownList = item.closest(".dropdown_menu").querySelector(".dropdown_list");
				if (!item.contains(event.target) && !dropdownList.classList.contains("d-none")) {
					dropdownList.classList.add("d-none");

					if (item.classList.contains("btn_language")) {
						const iconDrop = item.querySelector("span i");
						iconDrop.classList.remove("_180deg");
					}
				}
			});
		}
		catch { }

		// Huỷ khi mở phóng to ảnh
		try {
			const showImg = document.querySelector("#click_show_image");
			const img = showImg.querySelector(".box_show_img .img");
			const btnShowImgCancel = showImg.querySelector(".btnShowImgCancel");
			if (!showImg.classList.contains("d-none") && showImg.contains(event.target) && (!img.contains(event.target) || btnShowImgCancel.contains(event.target))) {
				img.style.cursor = "default";
				showImg.classList.add("d-none");
				document.body.style.overflow = "auto";
			}
		}
		catch { }

		// Huỷ mở menu hamburger
		try {
			const btnMenu = document.querySelector(".btn_menu");
			const iconS = btnMenu.querySelector(".icon-s");
			const listMenu = document.querySelector(".list_menu");

			if (!listMenu.contains(event.target) && !btnMenu.contains(event.target) && iconS.classList.contains("d-none")) {
				iconS.classList.remove("d-none");
				btnMenu.querySelector(".icon-x").classList.add("d-none");
				listMenu.classList.remove("right-0");
				document.body.style.overflow = "auto";
			}
		}
		catch { }

		// Huỷ mở QR code
		try {
			const showQRCode = document.querySelector("#show_qr_code");
			const wrapper = showQRCode.querySelector(".wrapper");
			const btnQRCancel = showQRCode.querySelector(".btnQR_cancel");

			if (!showQRCode.classList.contains("d-none") && showQRCode.contains(event.target) && (!wrapper.contains(event.target) || btnQRCancel.contains(event.target))) {
				showQRCode.classList.add("d-none");
			}
		}
		catch { }
	});

	// Event Keydown
	document.addEventListener("keydown", function (e) {
		try {
			if (e.key == "Escape") {
				const showImg = document.querySelector("#click_show_image");
				const img = showImg.querySelector(".box_show_img .img");
				if (!showImg.classList.contains("d-none")) {
					img.style.cursor = "default";
					showImg.classList.add("d-none");
					document.body.style.overflow = "auto";
				}

				const showQRCode = document.querySelector("#show_qr_code");
				if (!showQRCode.classList.contains("d-none")) {
					showQRCode.classList.add("d-none");
				}
			}
		}
		catch { }
	});
}
listenerClickToClose();

//---
console.log("Run file both_site.js");
