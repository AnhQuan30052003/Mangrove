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
		const tabss = document.querySelectorAll(".tabs");
		tabss.forEach((tab) => {
			if (tab.contains(e.target) && tab.classList.contains("have_move")) {
				new Sortable(tab, {
					animation: 150,
					ghostClass: "sortable-ghost",
					handle: ".tab_item",
					draggable: ".tab_item",
					onEnd: function (evt) {
						//let arr = new Array[5];
					},
				});
			}
		});
	});
}
changeIndexItemPhoto();
