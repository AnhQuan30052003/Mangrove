
// Cập nhật số liệu nhanh ra UI
function updateCount() {
	window.addEventListener("load", function () {
		const counters = document.querySelectorAll('.counter');

		counters.forEach((counter) => {
			let value = parseInt(counter.innerHTML);
			const target = parseInt(counter.getAttribute("data-target"));
			const valueAdd = parseInt(target) / 10;
			const update = function () {
				if (value < target) {
					value += valueAdd;
					counter.innerHTML = Math.floor(value).toLocaleString("vi-VN");
					setTimeout(update, 50);
				}
				else {
					counter.innerHTML = target.toLocaleString("vi-VN");
					clearTimeout(update);
				}
			};
			update();
		});
	});
}
updateCount();

window.addEventListener("load", function () {
	const table = document.getElementById("table_top_mangrove");
	const tableOffsetTop = table.offsetTop;

	const headers = table.querySelectorAll("th");

	headers.forEach((header) => {
		header.addEventListener("click", function (e) {
			setTimeout(() => {
				window.scrollTo({
					top: tableOffsetTop - 50,
					behavior: "smooth"
				});
			}, 200);
		});
	});
});
