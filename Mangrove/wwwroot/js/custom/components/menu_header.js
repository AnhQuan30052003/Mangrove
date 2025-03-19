// Theo dõi click hamburger menu
function clickHamburgerMenu() {
	try {
		const btnMenu = document.querySelector(".btn_menu");
		btnMenu.addEventListener("click", function () {
			const iconS = btnMenu.querySelector(".icon-s");
			const iconX = btnMenu.querySelector(".icon-x");
			const listMenu = document.querySelector(".list_menu");
			const bg = document.querySelector("#background");

			iconS.classList.toggle("d-none");
			iconX.classList.toggle("d-none");
			listMenu.classList.toggle("right-0");
			bg.classList.toggle("w-100");

			if (listMenu.classList.contains("right-0")) {
				document.body.style.overflow = "hidden";
			}
			else {
				document.body.style.overflow = "auto";
			}
		});
	}
	catch { }
}
clickHamburgerMenu();

// Theo dõ click dropdown_menu tự động
function toggleDropdownMenu() {
	const dropdownToggles = document.querySelectorAll(".dropdown_toggle");
	dropdownToggles.forEach((item) => {
		item.addEventListener("click", function (e) {
			const width = screen.width;
			if (width >= 1200) return;

			item.classList.toggle("highlight_item");

			const dropdownList = item.closest(".dropdown_menu").querySelector(".dropdown_list");
			if (dropdownList) dropdownList.classList.toggle("d-block");

			const iconDrop = item.querySelector("span i");
			if (iconDrop) iconDrop.classList.toggle("_180deg");
		});
	});
}
toggleDropdownMenu();