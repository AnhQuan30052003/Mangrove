// Thay đổi vị trí item photo
function changeIndexItemPhoto() {
	window.addEventListener("load", function () {
		// Cho các add photo item
		const listnerAddItemPhoto = setInterval(() => {
			let listItems = document.querySelectorAll(".items");
			let sort = null;
			listItems.forEach((item) => {
				if (item.classList.contains("have_move")) {
					if (sort instanceof Sortable) sort.destroy();
					sort = new Sortable(item, {
						animation: 150,
						ghostClass: "sortable-ghost",
						handle: ".catch_to_move",
						draggable: ".add_item",
						onEnd: function (evt) {

						},
					});
				}
			});
		}, 1000);

		// Cho các stage item
		const listenrStageItem = setInterval(() => {
			let tabsChange = document.querySelectorAll(".tabs_change");
			let sort = null;
			tabsChange.forEach((tabs) => {
				if (tabs.classList.contains("have_move")) {
					if (sort instanceof Sortable) sort.destroy();
					sort = new Sortable(tabs, {
						animation: 150,
						ghostClass: "sortable-ghost",
						handle: ".change_tab",
						draggable: ".change_tab",
						onEnd: function (evt) {
							const tabItems = tabs.querySelectorAll(".tab_item");
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
			});
		}, 1000);
	});
}
changeIndexItemPhoto();
