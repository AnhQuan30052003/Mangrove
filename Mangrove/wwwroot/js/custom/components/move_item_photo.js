// Thay đổi vị trí item photo
function changeIndexItemPhoto() {
	document.addEventListener("click", function (e) {
		// Cho các item image
		const itemss = document.querySelectorAll(".items");
		itemss.forEach((items) => {
			if (items.contains(e.target) && items.classList.contains("have_move")) {
				new Sortable(items, {
					animation: 150,
					ghostClass: "sortable-ghost",
					handle: ".add_img",
					draggable: ".add_item",
					onEnd: function (evt) {
						//console.log("Mục đã được kéo từ vị trí", evt.oldIndex, "đến", evt.newIndex);
					},
				});
			}
		});

		// Cho các item stage
		const tabsChange = document.querySelector(".tabs_change");
		if (tabsChange && tabsChange.classList.contains("have_move")) {
			new Sortable(tabsChange, {
				animation: 150,
				ghostClass: "sortable-ghost",
				handle: ".tab_item",
				draggable: ".tab_item",
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
	});
}
changeIndexItemPhoto();
