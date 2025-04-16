window.addEventListener("load", function () {
	updateCount();
	revoveryPositionPage();
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

// Lưu vị trí scroll trước khi trang reload
window.addEventListener("beforeunload", function () {
	localStorage.setItem("scrollPosition", window.scrollY);
});

// Cuộn tới vị trí sau khi reload
function revoveryPositionPage() {
	try {
		const savedPosition = localStorage.getItem("scrollPosition");
		if (savedPosition !== null) {
			window.scrollTo({
				top: parseInt(savedPosition, 10),
				behavior: "instant"
			});
			localStorage.removeItem("scrollPosition");
		}
	}
	catch { }
}
