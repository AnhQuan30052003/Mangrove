window.addEventListener("load", function () {
	updateCount();
	refershTable();
});

// Cập nhật số liệu nhanh ra UI
function updateCount() {
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
}

function refershTable () {
	const table = document.getElementById("table_top_mangrove");
	if (!table) return;

	const iconSorts = table.querySelectorAll(".icon_sort");
	iconSorts.forEach((icon) => {
		icon.addEventListener("click", function () {
			localStorage.setItem("scrollToTable", "true");
		});
	});

	// Nếu đã có vị trí được lưu, thì cuộn đến đó
	const shouldScrollToTable = localStorage.getItem("scrollToTable");
	if (shouldScrollToTable === "true") {
		localStorage.removeItem("scrollToTable");
		window.scrollTo({
			top: table.offsetTop,
			behavior: "instant"
		});
	}
}

