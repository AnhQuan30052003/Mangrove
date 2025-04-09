// Thay đổi vị trí item photo
function changeIndexItemPhoto() {
	document.addEventListener("click", function (e) {
		const clicked = e.target;

		// Cho các add photo item
		if (clicked.matches(".catch_to_move")) {
			const items = clicked.closest(".items");
			if (items.classList.contains("have_move")) {
				new Sortable(items, {
					animation: 150,
					ghostClass: "sortable-ghost",
					handle: ".catch_to_move",
					draggable: ".add_item",
					onEnd: function (evt) {

					},
				});
			}
		}

		// Cho các stage item
		if (clicked.matches(".change_tab") || clicked.matches(".value_tab")) {
			const tabsChange = clicked.closest(".tabs_change");
			if (tabsChange.classList.contains("have_move")) {
				new Sortable(tabsChange, {
					animation: 150,
					ghostClass: "sortable-ghost",
					handle: ".change_tab",
					draggable: ".change_tab",
					onEnd: function (evt) {
						const tabItems = tabsChange.querySelectorAll(".tab_item");
						const listDisplayItem = document.querySelectorAll(".display_item");

						const listIndex = Array.from(tabItems).map(tab => {
							const tabValue = tab.getAttribute("data-tab");
							return Array.from(listDisplayItem).find(item => item.getAttribute("data-tab") === tabValue);
						});

						const displayInfo = document.querySelector(".display_info_stage");
						displayInfo.innerHTML = listIndex.map(el => el.outerHTML).join("");
					},
				});
			}
		}
	});
}
changeIndexItemPhoto();
