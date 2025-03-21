// Thay đổi vị trí item photo
function changeIndexItemPhoto() {
	window.addEventListener("load", function () {
		const items = document.querySelector(".items");

		if (items) {
			new Sortable(items, {
				animation: 150,
				ghostClass: "sortable-ghost",
				handle: ".add_item",
				draggable: ".add_item",
				onEnd: function (evt) {
					//console.log("Mục đã được kéo từ vị trí", evt.oldIndex, "đến", evt.newIndex);
				},
			});
		}
	});
}
changeIndexItemPhoto();
